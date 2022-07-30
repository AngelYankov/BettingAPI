using BettingAPI.DataContext;
using BettingAPI.DataContext.Infrastructure;
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
                .Include(m => m.Bets)
                    .ThenInclude(b => b.Odds);

            var allMatchIds = allFutureMatches.Select(m => m.Id);

            var allBets = this.context.Bets.Where(
                b => allMatchIds.Contains(b.MatchId) &&
                b.IsActive == true &&
                (
                    b.Name == Constants.MatchWinner ||
                    b.Name == Constants.MapAdvantage ||
                    b.Name == Constants.TotalMapsPlayed)
                );

            var allBetIds = allBets.Select(b => b.Id);

            var allOdds = this.context.Odds.Where(o => allBetIds.Contains(o.BetId) && o.IsActive);

            var allOddIds = allOdds.Select(o => o.Id);

            foreach (var match in allFutureMatches)
            {
                match.Bets = match.Bets.Where(b => allBetIds.Contains(b.Id)).ToList();

                foreach (var bet in match.Bets)
                {
                    bet.Odds = bet.Odds
                        .Where(o => allOddIds.Contains(o.Id))
                        .GroupBy(b => b.SpecialValueBet)
                        .Select(grp => grp.ToList())
                        .First();
                }
            }

            return allFutureMatches.Select(m => new MatchWithBetsDTO(m)).ToList();
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
