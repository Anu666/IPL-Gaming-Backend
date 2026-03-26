using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IPL.Gaming.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;

        public MatchService(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<List<Match>> GetAllMatches()
        {
            return await _matchRepository.GetAllMatches();
        }

        public async Task<Match> GetMatchById(Guid matchId)
        {
            return await _matchRepository.GetMatchById(matchId);
        }

        public async Task<IEnumerable<Match>> GetMatchesByTeamName(string teamName)
        {
            return await _matchRepository.GetMatchesByTeamName(teamName);
        }

        public async Task<Match> CreateMatch(Match match)
        {
            // Preserve the provided ID; only generate a new one if none was given
            if (match.Id == Guid.Empty)
                match.Id = Guid.NewGuid();

            return await _matchRepository.CreateMatch(match);
        }

        public async Task<Match> UpdateMatch(Match match)
        {
            // Retrieve existing match
            var existingMatch = await _matchRepository.GetMatchById(match.Id);
            if (existingMatch == null)
            {
                throw new Exception($"Match with ID {match.Id} not found");
            }

            return await _matchRepository.UpdateMatch(match);
        }

        public async Task<bool> DeleteMatch(Guid matchId)
        {
            return await _matchRepository.DeleteMatch(matchId);
        }
    }
}
