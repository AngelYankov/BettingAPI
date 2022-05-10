using BettingAPI.DataContext.Models.Active;

namespace BettingAPI.Services.Models
{
    public class OddDTO
    {
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
