using System;
using System.Collections.Generic;

namespace BettingAPI.DataContext.Models
{
    public class Match
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public MatchType MatchType { get; set; }

        public List<Bet> Bets { get; set; }

        public bool IsActive { get; set; }
    }

    public enum MatchType
    {
        Prematch,
        Live,
        Outright
    }
}