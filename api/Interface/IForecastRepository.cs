using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface
{
    public interface IForecastRepository
    {
        Task<List<ForecastPoint>> GetForecastData();
        List<ForecastPoint> ForecastParser(string json);
    }
}