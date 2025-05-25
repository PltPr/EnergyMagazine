from fastapi import FastAPI, HTTPException
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

# Dane wejściowe
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

# Dodanie cech czasowych
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

        features = [
            "temperature", "windSpeed", "cloudCover", "precipitation",
            "relativeHumidity", "dewPoint",
            "dayofweek", "hour_sin", "hour_cos", "season", "isWeekend"
        ]

        # Upewnij się, że wszystkie kolumny istnieją
        missing = [col for col in features if col not in merged.columns]
        if missing:
            raise HTTPException(status_code=400, detail=f"Brak kolumn: {missing}")

        X = merged[features]
        y = merged["price"]

        param_grid = {
            'max_depth': [3, 4, 5],
            'learning_rate': [0.1, 0.05],
            'n_estimators': [100, 150],
            'subsample': [0.8, 1],
            'colsample_bytree': [0.8, 1],
            'min_child_weight': [2, 3],
            'gamma': [0, 0.05]
        }

        model = XGBRegressor(random_state=42, objective='reg:squarederror')
        grid = GridSearchCV(model, param_grid, cv=3, n_jobs=-1, verbose=1, scoring='neg_mean_squared_error')
        grid.fit(X, y)

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
