using IPL.Gaming.Common.Enums;

namespace IPL.Gaming.Common.Models.Requests
{
    public class UpdateMatchStatusRequest
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public MatchStatus Status { get; set; }
    }
}
