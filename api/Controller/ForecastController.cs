using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace api.Controller
{
    [Route("api/forecast")]
    [ApiController]
    public class ForecastController : ControllerBase
    {
        private readonly IForecastRepository _forecastRepo;
        private readonly IMemoryCache _cache;
        public ForecastController(IForecastRepository forecastRepo, IMemoryCache cache)
        {
            _forecastRepo = forecastRepo;
            _cache = cache;
        }

        [HttpGet("GetDataArchieve")]
        public async Task<IActionResult> GetForecastDataArchive()
        {
            var response = await _forecastRepo.GetForecastDataArchive();

            if (response == null || response.Count == 0)
                return NotFound();

            return Ok(response);
        }
        [HttpGet("GetDataPredict")]
        public async Task<IActionResult> GetForecastDataPredict()
        {
            var response = await _forecastRepo.GetForecastDataPredict();

            if (response == null || response.Count == 0)
                return NotFound();

            return Ok(response);
        }

    }
}