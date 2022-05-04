namespace BettingAPI.DataContext.Models
{
    public class Odd
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }

        public decimal? SpecialValueBet { get; set; }

        public int BetId { get; set; }

        public Bet Bet { get; set; }

        public bool IsActive { get => Bet.IsActive; set { } }
    }
}