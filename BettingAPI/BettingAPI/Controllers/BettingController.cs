using BettingAPI.Services;
using BettingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BettingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BettingController : ControllerBase
    {
        private readonly IBettingServiceNew bettingServiceNew;
        private readonly IMatchService matchService;

        public BettingController(IBettingServiceNew bettingServiceNew, IMatchService matchService)
        {
            this.bettingServiceNew = bettingServiceNew;
            this.matchService = matchService;
        }

        [HttpPost]
        public IActionResult UpdateData()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            this.bettingServiceNew.Save();
            s.Stop();
            return Ok(s.ElapsedMilliseconds / 1000);
        }

        [HttpGet]
        [Route("matches")]
        public IActionResult GetActiveMatches()
        {
            var matches = this.matchService.GetAllMatches();
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
