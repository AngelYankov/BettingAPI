using BettingAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BettingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IBettingOddsService bettingOddsService;

        public WeatherForecastController(IBettingOddsService bettingOddsService)
        {
            this.bettingOddsService = bettingOddsService;
        }

        [HttpPost]
        public IActionResult Get()
        {
            this.bettingOddsService.SaveData();
            return Ok();
        }
    }
}
