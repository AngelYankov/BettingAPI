using BettingAPI.DataContext.Models.History;

namespace BettingAPI.Services.Models
{
    public class BetHistoryDTO
    {
        public BetHistoryDTO(BetHistory betHistory)
        {
            this.Id = betHistory.Id;
            this.Name = betHistory.Name;
            this.IsLive = betHistory.IsLive;
            this.MatchHistoryId = betHistory.MatchHistoryId;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsLive { get; set; }

        public int MatchHistoryId { get; set; }
    }
}
