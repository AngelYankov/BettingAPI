using System;

namespace BettingAPI.DataContext.Models.ChangeLogs
{
    public class BetChangeLog
    {
        public int Id { get; set; }

        public int? BetXmlId { get; set; }

        public string Name { get; set; }

        public bool? IsLive { get; set; }

        public int? MatchId { get; set; }

        public string ActionType { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
