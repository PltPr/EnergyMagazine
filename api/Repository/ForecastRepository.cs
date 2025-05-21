using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using api.Interface;
using api.Models;

namespace api.Repository
{
    public class ForecastRepository : IForecastRepository
    {
        private readonly HttpClient _httpClient;
        public ForecastRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public List<ForecastPoint> ForecastParser(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            WeatherResponse weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(json, options);

            var points = new List<ForecastPoint>();

            if (weatherResponse?.hourly == null)
                return points;

            var hourly = weatherResponse.hourly;

            for (int i = 0; i < hourly.time.Count; i++)
            {
                points.Add(new ForecastPoint
                {
                    Date = DateTime.Parse(hourly.time[i]),
                    Temperature = hourly.temperature_2m[i],
                    WindSpeed = hourly.wind_speed_10m[i],
                    //SolarRadiation = hourly.solar_radiation[i],
                    CloudCover = hourly.cloudcover[i],
                    Precipitation = hourly.precipitation[i],
                    RelativeHumidity = hourly.relative_humidity_2m[i],
                    DewPoint = hourly.dew_point_2m[i]
                });
            }
            return points;

        }

        public async Task<List<ForecastPoint>> GetForecastData()
        {
            string startDate = "2023-05-01";
            string endDate = "2023-05-18";
            string url = $"https://archive-api.open-meteo.com/v1/archive" +
             $"?latitude=52.23" +
             $"&longitude=21.01" +
             $"&start_date={startDate}" +
             $"&end_date={endDate}" +
             $"&hourly=temperature_2m,wind_speed_10m,cloudcover,precipitation,relative_humidity_2m,dew_point_2m" +
             $"&timezone=Europe%2FWarsaw";


            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                var forecastPoints = ForecastParser(json);

                return forecastPoints;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bład pobieranie danych pogodowych: {ex.Message}");
                return new List<ForecastPoint>();
            }
        }
    }
}