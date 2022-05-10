using BettingAPI.DataContext.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models.History
{
    public class BetHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsLive { get; set; }

        public DateTime? MatchStartDate { get; set; }

        public MatchType? MatchType { get; set; }

        public int MatchHistoryId { get; set; }

        public MatchHistory MatchHistory { get; set; }

        public List<OddHistory> OddHistories { get; set; }

        public DateTime DateCreated { get; set; }
    }
}