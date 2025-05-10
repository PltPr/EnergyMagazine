using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using api.Interface;

namespace api.Repository
{
    public class EnergyRepository : IEnergyRepository
    {
        private readonly IConfiguration _configuration;
        public EnergyRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetEnergyDataForTodayAsync()
        {
        
            string apiKey = _configuration["EntsoE:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Brak klucza API w konfiguracji");

     
            string dateFrom = DateTime.UtcNow.ToString("yyyyMMdd") + "0000";
            string dateTo   = DateTime.UtcNow.ToString("yyyyMMdd") + "2300";

            
            string url = $"https://web-api.tp.entsoe.eu/api?" +
                         $"documentType=A44" +
                         $"&In_Domain=10YPL-AREA-----S" +
                         $"&Out_Domain=10YPL-AREA-----S" +
                         $"&periodStart={dateFrom}" +
                         $"&periodEnd={dateTo}" +
                         $"&securityToken={apiKey}";

            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Błąd podczas pobierania danych: {(int)response.StatusCode}");

            return await response.Content.ReadAsStringAsync();
        }
    }
}
