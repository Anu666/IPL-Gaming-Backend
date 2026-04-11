using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Common.Models.Requests;

namespace IPL.Gaming.Common.Mappers
{
    public static class MatchStatusMapper
    {
        public static MatchStatusRecord ToMatchStatusRecord(CreateMatchStatusRequest request) => new MatchStatusRecord
        {
            MatchId = request.MatchId,
            Status = request.Status,
            MatchCommenceStartDate = request.MatchCommenceStartDate
        };

        public static MatchStatusRecord ToMatchStatusRecord(UpdateMatchStatusRequest request) => new MatchStatusRecord
        {
            Id = request.Id,
            MatchId = request.MatchId,
            Status = request.Status
        };
    }
}
