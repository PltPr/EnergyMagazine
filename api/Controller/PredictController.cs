using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interface;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace api.Controller
{
    [Route("api/predict")]
    [ApiController]
    public class PredictController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClient;
        private readonly IForecastRepository _forecastRepo;
        private readonly IEnergyRepository _energyRepo;
        public PredictController(IMemoryCache cache, IHttpClientFactory httpClient, IForecastRepository forecastRepo, IEnergyRepository energyRepo)
        {
            _cache = cache;
            _httpClient = httpClient;
            _forecastRepo = forecastRepo;
            _energyRepo = energyRepo;
        }
        [HttpPost]
        public async Task<IActionResult> TrainModel()
        {
            string startDateF = "2025-01-15";
            string endDateF = "2025-04-30";
            var ForecastData = await _forecastRepo.GetForecastDataArchive(startDateF, endDateF);

            string startDateF24 = "2024-01-01";
            string endDateF24 = "2024-12-30";
            var ForecastData24 = await _forecastRepo.GetForecastDataArchive(startDateF24, endDateF24);

            var allForecastData = ForecastData.Concat(ForecastData24).ToList();

            string startDateE = "202501150000";
            string endDateE = "202501292300";
            var EnergyData = await _energyRepo.GetEnergyDataForTodayAsync(startDateE, endDateE);

            string startDateE24 = "202401010000";
            string endDateE24 = "202412292300";
            var EnergyData24 = await _energyRepo.GetEnergyDataForTodayAsync(startDateE24, endDateE24);

            var allEnergyData = EnergyData.Concat(EnergyData24).ToList();


            var trainData = new
            {
                forecast = ForecastData24,
                energy = EnergyData24
            };

            using var client = _httpClient.CreateClient();

            var response = await client.PostAsJsonAsync("http://localhost:8000/train", trainData);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TrainResponse>();
                Console.WriteLine($"Message: {result.message}");
                Console.WriteLine("Best params:");
                foreach (var param in result.best_params)
                    Console.WriteLine($"{param.Key}: {param.Value}");
                Console.WriteLine($"Best MSE: {result.best_mse}");
                Console.WriteLine($"Best RMSE: {result.best_rmse}");
                return Ok(result);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {error}");
                return BadRequest(error);
            }
        }
    }
}