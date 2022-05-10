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
        public List<AllMatchesDTO> GetAllMatches()
        {
            var allFutureMatches = this.context.MatchHistories
                .Where(m => m.StartDate > DateTime.Now && m.StartDate <= DateTime.Now.AddHours(24) && (int)m.MatchType != 2)
                .DistinctBy(m => m.Id)
                .ToList();

            var allActiveMatches = new List<AllMatchesDTO>();

            var allActiveBetsMatches = this.context.Matches
                .Include(m => m.Bets)
                    .ThenInclude(b => b.Odds)
                .ToList();

            foreach (var match in allFutureMatches)
            {
                var activeMatchBet = allActiveBetsMatches.FirstOrDefault(m => m.Id == match.Id);
                if (activeMatchBet != null)
                {
                    allActiveMatches.Add(new AllMatchesDTO(activeMatchBet));
                }
                else
                {
                    allActiveMatches.Add(new AllMatchesDTO(match));
                }
            }

            return allActiveMatches;
        }

        /// <summary>
        /// Gets a Match object according to a specific Id along with all of its active and past Bets and Odds
        /// </summary>
        /// <param name="matchXmlId">Id of the Match object according to the XML document</param>
        /// <returns>Match object with all of its active and past Bets and Odds</returns>
        public MatchDTO GetMatch(int matchXmlId)
        {
            var match = this.context.MatchHistories
                .Include(m => m.BetHistories)
                    .ThenInclude(b => b.OddHistories)
                .FirstOrDefault(m => m.Id == matchXmlId)
                ?? throw new ArgumentException(); ;

            var activeBets = this.context.Bets
                .Include(b => b.Odds)
                .Where(b => b.MatchId == matchXmlId)
                .ToList();

            var activeBetsDTO = activeBets
                .Select(b => new BetActiveDTO(b))
                .ToList();

            var activeBetsIds = activeBets.Select(b => b.Id);

            var allBets = this.context.BetHistories
                .Include(b => b.OddHistories)
                .Where(b => b.MatchHistoryId == matchXmlId)
                .ToList();

            allBets.RemoveAll(b => activeBetsIds.Contains(b.Id));

            var matchDto = new MatchDTO(match);
            matchDto.ActiveBets = activeBetsDTO;
            matchDto.PastBets = allBets.Select(b => new BetPastDTO(b)).ToList();

            return matchDto;
        }
    }
}
