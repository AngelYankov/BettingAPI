using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models
{
    public class Odd
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }

        public decimal? SpecialValueBet { get; set; }

        public int BetId { get; set; }

        public Bet Bet { get; set; }

        public bool IsActive { get; set; }
    }
}