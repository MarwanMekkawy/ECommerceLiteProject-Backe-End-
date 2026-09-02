namespace PaymentService.Application.DTOs
{
    public class StripeWebhookEventDto
    {
        public string EventId { get; set; } = string.Empty;
        public string RefundId { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? FailureReason { get; set; }
        public string? RefundStatus { get; set; }
    }
}
