using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using api.Interface;

namespace api.Repository
{
    public class EnergyRepository : IEnergyRepository
    {
        private readonly IConfiguration _configuration;
        public EnergyRepository(IConfiguration configuration)
        {
            _configuration=configuration;
        }
        public async Task<string> GetEnergyDataForTodayAsync()
        {
            string apiKey=_configuration["EntsoE:ApiKey"];
            if(string.IsNullOrEmpty(apiKey)) throw new Exception("Brak klucza api w konfiguracji");

            string date = DateTime.UtcNow.ToString("yyyyMMdd");

            return null;
        }
    }
}