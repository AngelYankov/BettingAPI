using BettingAPI.DataContext.Enums;
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
            this.EventHistoryId = match.EventHistoryId;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public MatchType MatchType { get; set; }

        public List<BetActiveDTO> ActiveBets { get; set; }

        public List<BetPastDTO> PastBets { get; set; }

        public int EventHistoryId { get; set; }
    }
}
