namespace IPL.Gaming.Services.Interfaces
{
    public interface IBettingStatsService
    {
        /// <summary>
        /// Calculates and persists betting stats for all questions in a match.
        /// Eligible pool = all active Player-role users.
        /// Unanswered questions auto-contribute to the loser pool.
        /// </summary>
        Task CalculateAndUpdateBettingStats(Guid matchId);
    }
}
