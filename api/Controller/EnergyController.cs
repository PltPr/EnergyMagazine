using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnergyController :ControllerBase
    {
        private readonly IConfiguration _configuration;
        public EnergyController(IConfiguration configuration)
        {
            _configuration=configuration;
        }

        [HttpGet("AllDayEnergy")]
        public async Task<IActionResult>GetAllDayEnergy()
        {
            return Ok();
        }
    }
}