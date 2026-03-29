using IPL.Gaming.Common.Enums;

namespace IPL.Gaming.Common.Models.Responses
{
    public class MatchStatusSummary
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public MatchStatus Status { get; set; }
    }
}
