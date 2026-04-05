using IPL.Gaming.Common.Enums;
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
        Task<MatchStatusRecord> MarkMatchComplete(Guid matchId);
        Task<MatchStatusRecord> MarkTransactionsSettled(Guid matchId);
        Task<MatchStatusRecord> MarkDone(Guid matchId);
        Task<MatchStatusRecord> MarkArchived(Guid matchId);
        Task<MatchStatusRecord> RecalculateLeaderboard(Guid matchId);
        Task<MatchStatusRecord> OverrideMatchStatus(Guid matchId, MatchStatus status);
        Task<bool> DeleteMatchStatus(Guid matchStatusId, Guid matchId);
    }
}
