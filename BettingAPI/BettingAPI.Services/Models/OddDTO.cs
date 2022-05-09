using BettingAPI.DataContext.Models.Active;
using BettingAPI.DataContext.Models.History;

namespace BettingAPI.Services.Models
{
    public class OddDTO
    {
        public OddDTO(OddHistory oddHistory)
        {
            this.Id = oddHistory.Id;
            this.Name = oddHistory.Name;
            this.Value = oddHistory.Value;
            this.BetId = oddHistory.BetHistoryId;
            this.SpecialValueBet = oddHistory.SpecialValueBet;
        }

        public OddDTO(Odd odd)
        {
            this.Id = odd.Id;
            this.Name = odd.Name;
            this.Value = odd.Value;
            this.BetId = odd.BetId;
            this.SpecialValueBet = odd.SpecialValueBet;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }

        public int BetId { get; set; }

        public string SpecialValueBet { get; set; }
    }
}
