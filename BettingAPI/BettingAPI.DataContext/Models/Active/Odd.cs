using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models.Active
{
    public class Odd
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }

        public string SpecialValueBet { get; set; }

        [ForeignKey("Bet")]
        public int BetId { get; set; }

        public Bet Bet { get; set; }
    }
}
