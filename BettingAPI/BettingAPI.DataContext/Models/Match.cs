using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models
{
    public class Match
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public MatchType MatchType { get; set; }

        public List<Bet> Bets { get; set; }

        public bool IsActive { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }

    public enum MatchType
    {
        PreMatch,
        Live,
        OutRight
    }
}