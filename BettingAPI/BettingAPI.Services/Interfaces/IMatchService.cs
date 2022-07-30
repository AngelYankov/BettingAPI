using BettingAPI.Services.Models;
using System.Collections.Generic;

namespace BettingAPI.Services.Interfaces
{
    public interface IMatchService
    {
        List<MatchWithBetsDTO> GetAllMatches();

        MatchWithBetsDTO GetMatch(int id);

    }
}