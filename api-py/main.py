from fastapi import Body, FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
from datetime import datetime
import pandas as pd
import joblib
import os
import numpy as np
from xgboost import XGBRegressor
from sklearn.model_selection import GridSearchCV

app = FastAPI()
MODEL_PATH = "model_xgb.pkl"

class ForecastPoint(BaseModel):
    date: datetime
    temperature: float
    windSpeed: float
    cloudCover: float
    precipitation: float
    relativeHumidity: float
    dewPoint: float

class EnergyPricePoint(BaseModel):
    dateTime: datetime
    price: float

class TrainingData(BaseModel):
    forecast: List[ForecastPoint]
    energy: List[EnergyPricePoint]

class ForecastInput(BaseModel):
    temperature: float
    windSpeed: float
    cloudCover: float
    precipitation: float
    relativeHumidity: float
    dewPoint: float
    dayofweek: int
    hour_sin: float
    hour_cos: float
    season: int
    isWeekend: int

def add_time_features(df: pd.DataFrame, date_col: str):
    dt = pd.to_datetime(df[date_col])
    df['dayofweek'] = dt.dt.dayofweek
    df['hour'] = dt.dt.hour
    df['hour_sin'] = np.sin(2 * np.pi * df['hour'] / 24)
    df['hour_cos'] = np.cos(2 * np.pi * df['hour'] / 24)
    df['isWeekend'] = df['dayofweek'].isin([5, 6]).astype(int)
    df['month'] = dt.dt.month
    df['season'] = df['month'].apply(lambda m: (m % 12) // 3)
    return df

@app.post("/train")
def train_model(data: TrainingData):
    try:
        forecast_df = pd.DataFrame([f.dict() for f in data.forecast])
        energy_df = pd.DataFrame([e.dict() for e in data.energy])

        forecast_df = add_time_features(forecast_df, "date")
        forecast_df["hour_rounded"] = pd.to_datetime(forecast_df["date"]).dt.floor("H")
        energy_df["hour_rounded"] = pd.to_datetime(energy_df["dateTime"]).dt.floor("H")

        merged = pd.merge(forecast_df, energy_df, on="hour_rounded", how="inner")

        if merged.empty:
            raise HTTPException(status_code=400, detail="Brak dopasowanych rekordów co godzinę")

        # Dodaj timestamp jako cechę
        merged["timestamp"] = pd.to_datetime(merged["hour_rounded"]).view('int64') / 1e9

        features = [
            "temperature", "windSpeed", "cloudCover", "precipitation",
            "relativeHumidity", "dewPoint",
            "dayofweek", "hour_sin", "hour_cos", "season", "isWeekend",
            "timestamp"
        ]

        missing = [col for col in features if col not in merged.columns]
        if missing:
            raise HTTPException(status_code=400, detail=f"Brak kolumn: {missing}")

        X = merged[features]
        y = merged["price"] + 30  # Dodaj korektę trendu

        weights = np.ones_like(y)

        peak_hours = merged['hour'].isin([6, 7, 8, 17, 18, 19, 20, 21])
        weights[peak_hours] = 3

        late_peak = merged['hour'].isin([20, 21])
        weights[late_peak] = 6

        # Sample weights: daj większe dla wartości skrajnych
        def weight_function(p):
            if p < 80 or p > 120:
                return 1
            elif p < 80 :
                return 1
            elif p > 140:
                return 1
            else:
                return 1

        sample_weights = merged["price"].apply(weight_function)

        # Model + GridSearch
        param_grid = {
            'max_depth': [3, 6],
            'learning_rate': [0.05, 0.1],
            'n_estimators': [100, 200],
            'subsample': [0.8, 1],
            'colsample_bytree': [0.8, 1],
            'min_child_weight': [1, 3],
            'gamma': [0, 0.1]
        }

        model = XGBRegressor(random_state=42, objective='reg:squarederror')
        grid = GridSearchCV(model, param_grid, cv=3, n_jobs=-1, verbose=1, scoring='neg_mean_squared_error')
        grid.fit(X, y, sample_weight=sample_weights)

        best_model = grid.best_estimator_
        joblib.dump(best_model, MODEL_PATH)

        return {
            "message": "Model wytrenowany i zapisany.",
            "best_params": grid.best_params_,
            "best_mse": -grid.best_score_,
            "best_rmse": (-grid.best_score_) ** 0.5
        }

    except HTTPException:
        raise
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Training error: {e}")

@app.post("/predict")
def predict(input: ForecastInput):
    if not os.path.exists(MODEL_PATH):
        raise HTTPException(status_code=400, detail="Model nie wytrenowany.")

    model = joblib.load(MODEL_PATH)
    df = pd.DataFrame([input.dict()])
    pred = model.predict(df)[0]
    return {"predicted_price": round(pred, 2)}

@app.post("/predict_batch")
async def predict_batch(inputs: List[ForecastPoint] = Body(...)):
    if not os.path.exists(MODEL_PATH):
        raise HTTPException(status_code=400, detail="Model nie wytrenowany.")

    model = joblib.load(MODEL_PATH)
    df = pd.DataFrame([input.dict() for input in inputs])
    df = add_time_features(df, "date")
    df["timestamp"] = pd.to_datetime(df["date"]).view('int64') / 1e9

    required_features = [
        "temperature", "windSpeed", "cloudCover", "precipitation",
        "relativeHumidity", "dewPoint", "dayofweek", "hour_sin",
        "hour_cos", "season", "isWeekend","timestamp"
    ]

    missing = [col for col in required_features if col not in df.columns]
    if missing:
        raise HTTPException(status_code=400, detail=f"Brak kolumn: {missing}")

    X = df[required_features]
    preds = model.predict(X)

    results = [
        {"date": input.date, "predicted_price": round(float(pred), 2)}
        for input, pred in zip(inputs, preds)
    ]
    return results
