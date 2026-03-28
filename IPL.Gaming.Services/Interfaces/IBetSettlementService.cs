using IPL.Gaming.Common.Models.CosmosDB;

namespace IPL.Gaming.Services.Interfaces
{
    public interface IBetSettlementService
    {
        /// <summary>
        /// Settles bets for a match. All questions must have correctOptionId set.
        /// Writes one Transaction per eligible user and stamps FinalStats on each question.
        /// Transitions match status to BetsSettled.
        /// </summary>
        Task SettleBets(Guid matchId);
    }
}
