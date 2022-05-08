using BettingAPI.DataContext.Models.History;

namespace BettingAPI.Services.Models
{
    public class OddHistoryDTO
    {
        public OddHistoryDTO(OddHistory oddHistory)
        {
            this.Id = oddHistory.Id;
            this.Name = oddHistory.Name;
            this.Value = oddHistory.Value;
            this.SpecialValueBet = oddHistory.SpecialValueBet;
            this.BetHistoryId = oddHistory.BetHistoryId;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }

        public string SpecialValueBet { get; set; }

        public int BetHistoryId { get; set; }
    }
}
