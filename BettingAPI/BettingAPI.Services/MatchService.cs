using BettingAPI.DataContext;
using BettingAPI.Services.Interfaces;
using BettingAPI.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BettingAPI.Services
{
    public class MatchService : IMatchService
    {
        private readonly BettingContext context;
        public MatchService(BettingContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets all Match objects starting in the next 24 hours along with all their active Bets and Odds
        /// </summary>
        /// <returns>List with all Match objects starting in the next 24 hours along with all their active Bets and Odds</returns>
        public List<MatchWithBetsDTO> GetAllMatches()
        {
            var allFutureMatches = this.context.Matches
                .Where(m => m.StartDate > DateTime.Now && m.StartDate <= DateTime.Now.AddHours(24) && (int)m.MatchType != 2)
                .Include(m=>m.Bets)
                    .ThenInclude(b=>b.Odds)
                .ToList();
            var allMatchIds = allFutureMatches.Select(m => m.Id);

            var allMatchBets = this.context.Bets.Where(
                b => allMatchIds.Contains(b.MatchId) && 
                b.IsActive == true && 
                (
                    b.Name == "Match Winner" || 
                    b.Name == "Map Advantage" || 
                    b.Name == "Total Maps Played")
                )
                .ToList();
            var allMatchBetIds = allMatchBets.Select(b => b.Id);

            var allBetOdds = this.context.Odds.Where(o => allMatchBetIds.Contains(o.BetId) && o.IsActive);
            var allBetOddsIds = allBetOdds.Select(o => o.Id);

            var all = allFutureMatches.Where(m => allMatchIds.Contains(m.Id));

            foreach (var match in all)
            {
                match.Bets = match.Bets.Where(b => allMatchBetIds.Contains(b.Id)).ToList();

                foreach (var bet in match.Bets)
                {
                    bet.Odds = bet.Odds.Where(o => allBetOddsIds.Contains(o.Id)).ToList().GroupBy(b => b.SpecialValueBet).Select(grp => grp.ToList()).First();
                }
            }

            return all.Select(m => new MatchWithBetsDTO(m)).ToList();
        }

        /// <summary>
        /// Gets a Match object according to a specific Id along with all of its active and past Bets and Odds
        /// </summary>
        /// <param name="matchXmlId">Id of the Match object according to the XML document</param>
        /// <returns>Match object with all of its active and past Bets and Odds</returns>
        public MatchWithBetsDTO GetMatch(int matchXmlId)
        {
            var match = this.context.Matches
                .Include(m => m.Bets)
                    .ThenInclude(b => b.Odds)
                .FirstOrDefault(m => m.Id == matchXmlId)
                ?? throw new ArgumentException();

            var matchDTO = new MatchWithBetsDTO(match);

            return matchDTO;
        }
    }
}
