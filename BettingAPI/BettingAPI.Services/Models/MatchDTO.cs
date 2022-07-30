using BettingAPI.DataContext.Enums;
using BettingAPI.DataContext.Models.Active;
using System;

namespace BettingAPI.Services.Models
{
    public class MatchDTO
    {
        public MatchDTO(Match match)
        {
            this.Id = match.Id;
            this.Name = match.Name;
            this.StartDate = match.StartDate;
            this.MatchType = match.MatchType;
            this.EventId = match.EventId;
            this.IsActive = match.IsActive;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public MatchType MatchType { get; set; }

        public int EventId { get; set; }

        public bool IsActive { get; set; }
    }
}
