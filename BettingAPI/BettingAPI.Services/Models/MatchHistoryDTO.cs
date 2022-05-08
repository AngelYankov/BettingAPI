using BettingAPI.DataContext.Models.History;
using System;

namespace BettingAPI.Services.Models
{
    public class MatchHistoryDTO
    {
        public MatchHistoryDTO(MatchHistory matchHistory)
        {
            this.Id = matchHistory.Id;
            this.Name = matchHistory.Name;
            this.StartDate = matchHistory.StartDate;
            this.MatchType = (int)matchHistory.MatchType;
            this.EventHistoryId = matchHistory.EventHistoryId;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public int MatchType { get; set; }

        public int EventHistoryId { get; set; }
    }
}
