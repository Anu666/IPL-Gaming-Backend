using IPL.Gaming.Common.Enums;

namespace IPL.Gaming.Common.Models.Requests
{
    public class CreateMatchStatusRequest
    {
        public Guid MatchId { get; set; }
        public MatchStatus Status { get; set; } = MatchStatus.NotStarted;
    }
}
