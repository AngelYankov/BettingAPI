using BettingAPI.DataContext.Models.History;
using System.Collections.Generic;
using System.Linq;

namespace BettingAPI.Services.Models
{
    public class BetPastDTO
    {
        public BetPastDTO(BetHistory betHistory)
        {
            this.Id = betHistory.Id;
            this.IsLive = betHistory.IsLive;
            this.MatchId = betHistory.MatchHistoryId;
            this.Name = betHistory.Name;
            this.Odds = betHistory.OddHistories.Select(o => new OddHistoryDTO(o)).ToList();
        }

        public int Id { get; set; }

        public bool IsLive { get; set; }

        public int MatchId { get; set; }

        public string Name { get; set; }

        public List<OddHistoryDTO> Odds { get; set; }
    }
}