namespace IPL.Gaming.Common.Models.Requests
{
    public class CreateMatchRequest
    {
        public DateTime MatchDate { get; set; }
        public string MatchName { get; set; }
        public string MatchTime { get; set; }
        public string GMTMatchTime { get; set; }
        public string GMTMatchDate { get; set; }
        public string GMTMatchEndTime { get; set; }
        public string GMTMatchEndDate { get; set; }
        public int FirstBattingTeamID { get; set; }
        public string FirstBattingTeamName { get; set; }
        public int SecondBattingTeamID { get; set; }
        public string SecondBattingTeamName { get; set; }
        public string FirstBattingTeamCode { get; set; }
        public string SecondBattingTeamCode { get; set; }
        public int GroundID { get; set; }
        public string GroundName { get; set; }
        public DateTime MatchCommenceStartDate { get; set; }
        public string City { get; set; }
        public int HomeTeamID { get; set; }
        public string HomeTeamName { get; set; }
        public int AwayTeamID { get; set; }
        public string AwayTeamName { get; set; }
    }
}
