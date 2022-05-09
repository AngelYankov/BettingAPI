using BettingAPI.Services.Models;
using System.Collections.Generic;

namespace BettingAPI.Services.Interfaces
{
    public interface IMatchService
    {
        List<AllMatchesDTO> GetAllMatches();

        MatchDTO GetMatch(int id);

    }
}