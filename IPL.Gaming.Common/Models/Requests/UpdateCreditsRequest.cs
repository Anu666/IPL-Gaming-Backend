namespace IPL.Gaming.Common.Models.Requests
{
    public enum CreditsOperation
    {
        Deposit = 0,
        Withdrawal = 1,
        Override = 2
    }

    public class UpdateCreditsRequest
    {
        public float Credits { get; set; }
        public CreditsOperation Operation { get; set; } = CreditsOperation.Deposit;
    }
}
