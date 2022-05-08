using System.ComponentModel.DataAnnotations.Schema;

namespace BettingAPI.DataContext.Models.History
{
    public class OddHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }

        public string SpecialValueBet { get; set; }

        public int BetHistoryId { get; set; }

        public BetHistory BetHistory { get; set; }
    }
}