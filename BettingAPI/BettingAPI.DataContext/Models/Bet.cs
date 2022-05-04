using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models
{
    public class Bet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsLive { get; set; }

        public int MatchId { get; set; }

        public Match Match { get; set; }

        public List<Odd> Odds { get; set; }

        public bool IsActive { get; set; }
    }
}