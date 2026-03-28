using IPL.Gaming.Common.Models.CosmosDB;

namespace IPL.Gaming.Services.Interfaces
{
    public interface ILeaderboardService
    {
        Task<List<LeaderboardEntry>> CalculateCumulativeLeaderboard(Guid currentMatchId);
    }
}
