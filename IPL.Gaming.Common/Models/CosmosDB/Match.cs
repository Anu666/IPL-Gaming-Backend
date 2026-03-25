using Newtonsoft.Json;
using System;

namespace IPL.Gaming.Common.Models.CosmosDB
{
    public class Match
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "matchDate")]
        public DateTime MatchDate { get; set; }

        [JsonProperty(PropertyName = "matchName")]
        public string MatchName { get; set; }

        [JsonProperty(PropertyName = "matchTime")]
        public string MatchTime { get; set; }

        [JsonProperty(PropertyName = "gmtMatchTime")]
        public string GMTMatchTime { get; set; }

        [JsonProperty(PropertyName = "gmtMatchDate")]
        public string GMTMatchDate { get; set; }

        [JsonProperty(PropertyName = "gmtMatchEndTime")]
        public string GMTMatchEndTime { get; set; }

        [JsonProperty(PropertyName = "gmtMatchEndDate")]
        public string GMTMatchEndDate { get; set; }

        [JsonProperty(PropertyName = "firstBattingTeamID")]
        public int FirstBattingTeamID { get; set; }

        [JsonProperty(PropertyName = "firstBattingTeamName")]
        public string FirstBattingTeamName { get; set; }

        [JsonProperty(PropertyName = "secondBattingTeamID")]
        public int SecondBattingTeamID { get; set; }

        [JsonProperty(PropertyName = "secondBattingTeamName")]
        public string SecondBattingTeamName { get; set; }

        [JsonProperty(PropertyName = "firstBattingTeamCode")]
        public string FirstBattingTeamCode { get; set; }

        [JsonProperty(PropertyName = "secondBattingTeamCode")]
        public string SecondBattingTeamCode { get; set; }

        [JsonProperty(PropertyName = "groundID")]
        public int GroundID { get; set; }

        [JsonProperty(PropertyName = "groundName")]
        public string GroundName { get; set; }

        [JsonProperty(PropertyName = "matchCommenceStartDate")]
        public DateTime MatchCommenceStartDate { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "homeTeamID")]
        public int HomeTeamID { get; set; }

        [JsonProperty(PropertyName = "homeTeamName")]
        public string HomeTeamName { get; set; }

        [JsonProperty(PropertyName = "awayTeamID")]
        public int AwayTeamID { get; set; }

        [JsonProperty(PropertyName = "awayTeamName")]
        public string AwayTeamName { get; set; }
    }
}
