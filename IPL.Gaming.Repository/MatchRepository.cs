using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Database.Interfaces;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Store;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IPL.Gaming.Repository
{
    public class MatchRepository : IMatchRepository
    {
        private readonly ICosmosService _cosmosService;
        private readonly string containerName = DataStore.Match;

        public MatchRepository(ICosmosService cosmosService)
        {
            _cosmosService = cosmosService;
        }

        public async Task<List<Match>> GetAllMatches()
        {
            var queryString = "SELECT * FROM M";
            var queryDefinition = new QueryDefinition(queryString);

            var matches = await _cosmosService.GetItemsAsync<Match>(containerName, queryDefinition);
            return matches.ToList();
        }

        public async Task<Match> GetMatchById(Guid matchId)
        {
            var queryString = "SELECT * FROM M WHERE M.id = @matchId";
            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@matchId", matchId.ToString());

            var matches = await _cosmosService.GetItemsAsync<Match>(containerName, queryDefinition);
            return matches.FirstOrDefault();
        }
        public async Task<IEnumerable<Match>> GetMatchesByTeamName(string teamName)
        {
            var query = @"SELECT * FROM M WHERE 
                CONTAINS(LOWER(M.firstBattingTeamName), LOWER(@teamName)) OR 
                CONTAINS(LOWER(M.secondBattingTeamName), LOWER(@teamName)) OR 
                CONTAINS(LOWER(M.homeTeamName), LOWER(@teamName)) OR 
                CONTAINS(LOWER(M.awayTeamName), LOWER(@teamName)) OR
                CONTAINS(LOWER(M.firstBattingTeamCode), LOWER(@teamName)) OR 
                CONTAINS(LOWER(M.secondBattingTeamCode), LOWER(@teamName))";
            
            var queryDefinition = new QueryDefinition(query)
                .WithParameter("@teamName", teamName);

            var matches = await _cosmosService.GetItemsAsync<Match>(DataStore.Match, queryDefinition);
            return matches;
        }
        public async Task<Match> CreateMatch(Match match)
        {
            var createdMatch = await _cosmosService.AddItemAsync(containerName, match);
            return createdMatch;
        }

        public async Task<Match> UpdateMatch(Match match)
        {
            var updatedMatch = await _cosmosService.UpsertItemAsync(containerName, match, match.Id.ToString());
            return updatedMatch;
        }

        public async Task<bool> DeleteMatch(Guid matchId)
        {
            try
            {
                var matchIdString = matchId.ToString();
                await _cosmosService.DeleteItemAsync<Match>(containerName, matchIdString, matchIdString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
