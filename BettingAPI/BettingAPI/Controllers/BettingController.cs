using BettingAPI.Services;
using BettingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;

namespace BettingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BettingController : ControllerBase
    {
        private readonly IBettingService bettingServiceNew;
        private readonly IMatchService matchService;

        public BettingController(IBettingService bettingServiceNew, IMatchService matchService)
        {
            this.bettingServiceNew = bettingServiceNew;
            this.matchService = matchService;
        }

        /// <summary>
        /// Test endpoint for saving data from XML document
        /// </summary>
        /// <returns>Time elapsed to retrieve and save data from XML document</returns>
        [HttpPost]
        [Route("save")]
        public IActionResult UpdateData()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            this.bettingServiceNew.Save();
            s.Stop();
            return Ok(s.ElapsedMilliseconds / 1000);
        }

        /// <summary>
        /// Get all Mathes starting in the next 24 hours along with all their active Bets and Odds
        /// </summary>
        /// <returns>Returns all Mathes starting in the next 24 hours along with all their active Bets and Odds 
        /// or just the Match info if no active Bets and Odds</returns>
        [HttpGet]
        [Route("matches")]
        public IActionResult GetActiveMatches()
        {
            var matches = this.matchService.GetAllMatches();
            return Ok(matches);
        }

        /// <summary>
        /// Get Match by Id from Xml
        /// </summary>
        /// <param name="matchXmlId">Id of the Match from the XML document</param>
        /// <returns>Returns Match object with all active and past Bets and Odds 
        /// or 404 NotFound response if no Match object with such Id exists</returns>
        [HttpGet]
        [Route("matches/{matchXmlId}")]
        public IActionResult GetMatch(int matchXmlId)
        {
            try
            {
                var match = this.matchService.GetMatch(matchXmlId);
                return Ok(match);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
