using IPL.Gaming.Common.Models.CosmosDB;

namespace IPL.Gaming.Services.Interfaces
{
    public interface IMatchStatusService
    {
        Task<List<MatchStatusRecord>> GetAllMatchStatuses();
        Task<MatchStatusRecord> GetMatchStatusById(Guid matchStatusId);
        Task<MatchStatusRecord> GetMatchStatusByMatchId(Guid matchId);
        Task<MatchStatusRecord> CreateMatchStatus(MatchStatusRecord matchStatus);
        Task<MatchStatusRecord> UpdateMatchStatus(MatchStatusRecord matchStatus);
        Task<bool> DeleteMatchStatus(Guid matchStatusId, Guid matchId);
    }
}
