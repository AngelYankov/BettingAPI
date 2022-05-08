using BettingAPI.DataContext.Models.Active;
using BettingAPI.DataContext.Models.History;
using System.Collections.Generic;
using System.Linq;

namespace BettingAPI.Services.Models
{
    public class BetDTO
    {
        public BetDTO(BetHistory betHistory)
        {
            //this.Id = betHistory.Id;
            this.IsLive = betHistory.IsLive;
            this.MatchId = betHistory.MatchHistoryId;
            this.Name = betHistory.Name;
            this.Odds = betHistory.OddHistories.Select(o => new OddDTO(o)).ToList();
        }

        public BetDTO(Bet bet)
        {
            this.Id = bet.Id;
            this.IsLive = bet.IsLive;
            this.MatchId = bet.MatchId;
            this.Name = bet.Name;
            this.Odds = bet.Odds.Select(o => new OddDTO(o)).ToList();
        }

        public int Id { get; set; }

        public bool IsLive { get; set; }

        public int MatchId { get; set; }

        public string Name { get; set; }

        public List<OddDTO> Odds { get; set; }
    }
}
