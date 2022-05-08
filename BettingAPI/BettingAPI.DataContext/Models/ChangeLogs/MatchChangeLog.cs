using System;

namespace BettingAPI.DataContext.Models
{
    public class MatchChangeLog
    {
        public int Id { get; set; }

        public int? MatchXmlId { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public MatchType? MatchType { get; set; }

        public int? EventId { get; set; }

        public string ActionType { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }

    public enum MatchType
    {
        PreMatch,
        Live,
        OutRight
    }
}
