using BettingAPI.Services;
using BettingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BettingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IBettingOddsService bettingOddsService;
        private readonly IMatchService matchService;
        private readonly IBettingService bettingService;
        private readonly IBettingServiceNew bettingServiceNew;

        public WeatherForecastController(IBettingOddsService bettingOddsService, IMatchService matchService, IBettingService bettingService, IBettingServiceNew bettingServiceNew)
        {
            this.bettingOddsService = bettingOddsService;
            this.matchService = matchService;
            this.bettingService = bettingService;
            this.bettingServiceNew = bettingServiceNew;
        }

        [HttpPost]
        public IActionResult Get()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            this.bettingOddsService.Save();
            s.Stop();
            return Ok(s.ElapsedMilliseconds/1000);
        }

        [HttpPost]
        [Route("New")]
        public IActionResult GetNew()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            this.bettingServiceNew.Save();
            s.Stop();
            return Ok(s.ElapsedMilliseconds / 1000);
        }

        [HttpGet]
        [Route("Home")]
        public IActionResult Update()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            this.bettingService.SaveData();
            s.Stop();
            return Ok(s.ElapsedMilliseconds / 1000);
        }

        [HttpGet]
        [Route("matches")]
        public IActionResult GetActiveMatches()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            var matches = this.matchService.GetAllMatches();
            s.Stop();
            return Ok(matches);
        }

        [HttpGet]
        [Route("matches/{id}")]
        public IActionResult GetMatch(int id)
        {
            var match = this.matchService.GetMatch(id);
            return Ok(match);
        }
    }
}
