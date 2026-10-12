namespace Gateway.API.DTOs
{
    public class RetryPaymentResponseDto
    {
        public Guid PaymentId { get; set; }
        public string ClientSecret { get; set; } = null!;
        public PaymentStatus Status { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        RequiresAction,
        Processing,
        Succeeded,
        Failed,
        Cancelled,

        RefundInitiated,
        RefundingByStripe,
        Refunded,
        RefundFailed
    }
}
