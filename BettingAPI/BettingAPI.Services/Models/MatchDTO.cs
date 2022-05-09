using BettingAPI.DataContext.Models.History;
using System;
using System.Collections.Generic;

namespace BettingAPI.Services.Models
{
    public class MatchDTO
    {
        public MatchDTO(MatchHistory match)
        {
            this.Id = match.Id;
            this.Name = match.Name;
            this.StartDate = match.StartDate;
            this.MatchType = match.MatchType;
            this.EventId = match.EventHistoryId;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public MatchType MatchType { get; set; }

        public List<BetDTO> ActiveBets { get; set; }

        public List<BetDTO> PastBets { get; set; }

        public int EventId { get; set; }
    }
}
