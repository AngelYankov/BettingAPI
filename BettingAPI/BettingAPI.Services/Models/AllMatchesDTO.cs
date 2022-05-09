using BettingAPI.DataContext.Enums;
using BettingAPI.DataContext.Models.Active;
using BettingAPI.DataContext.Models.History;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BettingAPI.Services.Models
{
    public class AllMatchesDTO
    {
        public AllMatchesDTO(MatchHistory matchHistory)
        {
            this.Id = matchHistory.Id;
            this.Name = matchHistory.Name;
            this.StartDate = matchHistory.StartDate;
            this.MatchType = matchHistory.MatchType;
            this.EventId = matchHistory.EventHistoryId;
        }

        public AllMatchesDTO(Match match)
        {
            this.Id = match.Id;
            this.Name = match.Name;
            this.StartDate = match.StartDate;
            this.MatchType = match.MatchType;
            this.Bets = match.Bets.Select(b => new BetDTO(b)).ToList();
            this.EventId = match.EventId;
            this.Event = match.Event;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public MatchType MatchType { get; set; }

        public List<BetDTO> Bets { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }
}
