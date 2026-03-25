using Newtonsoft.Json;

namespace IPL.Gaming.MatchSchedule
{
    public class MatchScheduleRoot
    {
        [JsonProperty("Matchsummary")]
        public List<MatchScheduleItem> Matchsummary { get; set; }
    }

    public class MatchScheduleItem
    {
        public string MatchDate { get; set; }
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
        
        [JsonProperty("MATCH_COMMENCE_START_DATE")]
        public string MatchCommenceStartDate { get; set; }
        
        [JsonProperty("city")]
        public string City { get; set; }
        
        [JsonProperty("HomeTeamID")]
        public string HomeTeamID { get; set; }
        
        [JsonProperty("HomeTeamName")]
        public string HomeTeamName { get; set; }
        
        [JsonProperty("AwayTeamID")]
        public string AwayTeamID { get; set; }
        
        [JsonProperty("AwayTeamName")]
        public string AwayTeamName { get; set; }
    }
}
