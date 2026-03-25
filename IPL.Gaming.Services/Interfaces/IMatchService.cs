using IPL.Gaming.Common.Models.CosmosDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPL.Gaming.Services.Interfaces
{
    public interface IMatchService
    {
        Task<List<Match>> GetAllMatches();
        Task<Match> GetMatchById(Guid matchId);        Task<IEnumerable<Match>> GetMatchesByTeamName(string teamName);        Task<Match> CreateMatch(Match match);
        Task<Match> UpdateMatch(Match match);
        Task<bool> DeleteMatch(Guid matchId);
    }
}
