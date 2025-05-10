using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interface;
using Microsoft.AspNetCore.Mvc;

namespace api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnergyController :ControllerBase
    {
        private readonly IEnergyRepository _energyrepo;
        public EnergyController(IEnergyRepository energyrepo)
        {
            _energyrepo=energyrepo;
        }

        [HttpGet("AllDayEnergy")]
        public async Task<IActionResult>GetAllDayEnergy()
        {
            var response=await _energyrepo.GetEnergyDataForTodayAsync();

            return Ok(response);
        }
    }
}