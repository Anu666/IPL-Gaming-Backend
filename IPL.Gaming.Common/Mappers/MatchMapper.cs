using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Common.Models.Requests;

namespace IPL.Gaming.Common.Mappers
{
    public static class MatchMapper
    {
        public static Match ToMatch(CreateMatchRequest request) => new Match
        {
            MatchDate = request.MatchDate,
            MatchName = request.MatchName,
            MatchTime = request.MatchTime,
            GMTMatchTime = request.GMTMatchTime,
            GMTMatchDate = request.GMTMatchDate,
            GMTMatchEndTime = request.GMTMatchEndTime,
            GMTMatchEndDate = request.GMTMatchEndDate,
            FirstBattingTeamID = request.FirstBattingTeamID,
            FirstBattingTeamName = request.FirstBattingTeamName,
            SecondBattingTeamID = request.SecondBattingTeamID,
            SecondBattingTeamName = request.SecondBattingTeamName,
            FirstBattingTeamCode = request.FirstBattingTeamCode,
            SecondBattingTeamCode = request.SecondBattingTeamCode,
            GroundID = request.GroundID,
            GroundName = request.GroundName,
            MatchCommenceStartDate = request.MatchCommenceStartDate,
            City = request.City,
            HomeTeamID = request.HomeTeamID,
            HomeTeamName = request.HomeTeamName,
            AwayTeamID = request.AwayTeamID,
            AwayTeamName = request.AwayTeamName
        };

        public static Match ToMatch(UpdateMatchRequest request) => new Match
        {
            Id = request.Id,
            MatchDate = request.MatchDate,
            MatchName = request.MatchName,
            MatchTime = request.MatchTime,
            GMTMatchTime = request.GMTMatchTime,
            GMTMatchDate = request.GMTMatchDate,
            GMTMatchEndTime = request.GMTMatchEndTime,
            GMTMatchEndDate = request.GMTMatchEndDate,
            FirstBattingTeamID = request.FirstBattingTeamID,
            FirstBattingTeamName = request.FirstBattingTeamName,
            SecondBattingTeamID = request.SecondBattingTeamID,
            SecondBattingTeamName = request.SecondBattingTeamName,
            FirstBattingTeamCode = request.FirstBattingTeamCode,
            SecondBattingTeamCode = request.SecondBattingTeamCode,
            GroundID = request.GroundID,
            GroundName = request.GroundName,
            MatchCommenceStartDate = request.MatchCommenceStartDate,
            City = request.City,
            HomeTeamID = request.HomeTeamID,
            HomeTeamName = request.HomeTeamName,
            AwayTeamID = request.AwayTeamID,
            AwayTeamName = request.AwayTeamName
        };
    }
}
