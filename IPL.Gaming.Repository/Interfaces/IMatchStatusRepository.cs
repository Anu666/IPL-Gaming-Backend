using IPL.Gaming.Common.Models.CosmosDB;

namespace IPL.Gaming.Repository.Interfaces
{
    public interface IMatchStatusRepository
    {
        Task<List<MatchStatusRecord>> GetAllMatchStatuses();
        Task<MatchStatusRecord> GetMatchStatusById(Guid matchStatusId);
        Task<MatchStatusRecord> GetMatchStatusByMatchId(Guid matchId);
        Task<MatchStatusRecord> CreateMatchStatus(MatchStatusRecord matchStatus);
        Task<MatchStatusRecord> UpdateMatchStatus(MatchStatusRecord matchStatus);
        Task<bool> DeleteMatchStatus(Guid matchStatusId, Guid matchId);
    }
}
