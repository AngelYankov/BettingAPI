using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models.Active
{
    public class Bet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsLive { get; set; }

        [ForeignKey("Match")]
        public int MatchId { get; set; }

        public Match Match { get; set; }

        public List<Odd> Odds { get; set; }
    }
}
