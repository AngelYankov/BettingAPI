using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BettingAPI.DataContext.Enums;

namespace BettingAPI.DataContext.Models.History
{
    public class MatchHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public MatchType MatchType { get; set; }

        public List<BetHistory> BetHistories { get; set; }

        [ForeignKey("EventHistory")]
        public int EventHistoryId { get; set; }

        public EventHistory EventHistory { get; set; }
    }
}