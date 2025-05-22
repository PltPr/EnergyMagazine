using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface
{
    public interface IForecastRepository
    {
        Task<List<ForecastPoint>> GetForecastDataArchive(string? startDate=null, string? endDate=null);
        Task<List<ForecastPoint>> GetForecastDataPredict(string? startDate=null, string? endDate=null);
        List<ForecastPoint> ForecastParser(string json);
    }
}