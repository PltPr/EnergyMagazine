using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface
{
    public interface IEnergyRepository
    {
        Task<List<EnergyPricePoint>>GetEnergyDataForTodayAsync(string? startDate=null, string? endDate=null);
        public List<EnergyPricePoint> ParseXml(string xml);
    }
}