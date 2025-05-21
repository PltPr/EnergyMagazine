using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interface;
using Microsoft.AspNetCore.Mvc;

namespace api.Controller
{
    [Route("api/forecast")]
    [ApiController]
    public class ForecastController : ControllerBase
    {
        private readonly IForecastRepository _forecastRepo;
        public ForecastController(IForecastRepository forecastRepo)
        {
            _forecastRepo = forecastRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetForecastData()
        {
            var response = await _forecastRepo.GetForecastData();
            if (response==null||response.Count==0) return NotFound();

            return Ok(response);
        }
    }
}