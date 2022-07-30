using BettingAPI.DataContext.Enums;
using BettingAPI.DataContext.Models.Active;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BettingAPI.Services.Models
{
    public class MatchWithBetsDTO
    {
        public MatchWithBetsDTO(Match match)
        {
            this.Id = match.Id;
            this.Name = match.Name;
            this.StartDate = match.StartDate;
            this.MatchType = match.MatchType;
            this.AllBets = match.Bets.Select(b => new BetDTO(b)).ToList();
            this.EventId = match.EventId;
            this.Event = match.Event;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public MatchType MatchType { get; set; }

        public List<BetDTO> AllBets { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }
}
