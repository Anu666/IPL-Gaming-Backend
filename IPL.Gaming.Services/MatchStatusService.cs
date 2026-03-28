using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Repository.Interfaces;
using IPL.Gaming.Services.Interfaces;

namespace IPL.Gaming.Services
{
    public class MatchStatusService : IMatchStatusService
    {
        private readonly IMatchStatusRepository _matchStatusRepository;

        public MatchStatusService(IMatchStatusRepository matchStatusRepository)
        {
            _matchStatusRepository = matchStatusRepository;
        }

        public async Task<List<MatchStatusRecord>> GetAllMatchStatuses()
        {
            return await _matchStatusRepository.GetAllMatchStatuses();
        }

        public async Task<MatchStatusRecord> GetMatchStatusById(Guid matchStatusId)
        {
            return await _matchStatusRepository.GetMatchStatusById(matchStatusId);
        }

        public async Task<MatchStatusRecord> GetMatchStatusByMatchId(Guid matchId)
        {
            return await _matchStatusRepository.GetMatchStatusByMatchId(matchId);
        }

        public async Task<MatchStatusRecord> CreateMatchStatus(MatchStatusRecord matchStatus)
        {
            matchStatus.Id = matchStatus.MatchId;
            return await _matchStatusRepository.CreateMatchStatus(matchStatus);
        }

        public async Task<MatchStatusRecord> UpdateMatchStatus(MatchStatusRecord matchStatus)
        {
            var existing = await _matchStatusRepository.GetMatchStatusById(matchStatus.Id);
            if (existing == null)
                throw new Exception($"MatchStatus with ID {matchStatus.Id} not found");

            return await _matchStatusRepository.UpdateMatchStatus(matchStatus);
        }

        public async Task<bool> DeleteMatchStatus(Guid matchStatusId, Guid matchId)
        {
            return await _matchStatusRepository.DeleteMatchStatus(matchStatusId, matchId);
        }

        public async Task<MatchStatusRecord> MarkMatchComplete(Guid matchId)
        {
            var existing = await _matchStatusRepository.GetMatchStatusByMatchId(matchId);
            if (existing == null)
                throw new Exception($"No status record found for match {matchId}");

            if (existing.Status != MatchStatus.BetsUpdated)
                throw new InvalidOperationException($"Match status must be 'BetsUpdated' to mark as complete. Current status: {existing.Status}");

            existing.Status = MatchStatus.MatchCompleted;
            return await _matchStatusRepository.UpdateMatchStatus(existing);
        }

        public async Task<MatchStatusRecord> OverrideMatchStatus(Guid matchId, MatchStatus status)
        {
            var existing = await _matchStatusRepository.GetMatchStatusByMatchId(matchId);
            if (existing == null)
                throw new Exception($"No status record found for match {matchId}");

            existing.Status = status;
            return await _matchStatusRepository.UpdateMatchStatus(existing);
        }
    }
}
