namespace IPL.Gaming.Common.Models.Requests
{
    public enum CreditsOperation
    {
        Override = 0,
        Increase = 1
    }

    public class UpdateCreditsRequest
    {
        public float Credits { get; set; }
        public CreditsOperation Operation { get; set; } = CreditsOperation.Override;
    }
}
