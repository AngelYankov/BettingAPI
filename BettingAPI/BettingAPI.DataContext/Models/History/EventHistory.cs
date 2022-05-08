using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models.History
{
    public class EventHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsLive { get; set; }

        public int CategoryID { get; set; }

        public List<MatchHistory> MatchHistories { get; set; }

        [ForeignKey("SportHistory")]
        public int SportHistoryId { get; set; }

        public SportHistory SportHistory { get; set; }
    }
}