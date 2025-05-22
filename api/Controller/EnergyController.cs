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
    [Route("api/energy")]
    [ApiController]
    public class EnergyController : ControllerBase
    {
        private readonly IEnergyRepository _energyrepo;
        private readonly IMemoryCache _cache;
        public EnergyController(IEnergyRepository energyrepo, IMemoryCache cache)
        {
            _energyrepo = energyrepo;
            _cache = cache;
        }

        [HttpGet("AllDayEnergy")]
        public async Task<IActionResult> GetAllDayEnergy()
        {
            var response = await _energyrepo.GetEnergyDataForTodayAsync();

            _cache.Set("TodayEnergyData", response, TimeSpan.FromHours(1));

            return Ok(response);
        }
        [HttpGet("Extremes")]
        public IActionResult GetExtremes()
        {
            if (!_cache.TryGetValue("TodayEnergyData", out List<EnergyPricePoint> cachedData))
            {
                return NotFound("No data");
            }
            var lowestPrice = cachedData.GroupBy(x => x.DateTime.Date)
            .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"),
            g => g.OrderBy(x => x.Price).Take(2).ToList()
            );
            var result = lowestPrice.SelectMany(k => k.Value).OrderBy(x => x.DateTime).ToList();

            return Ok(result);
        }
    }
}