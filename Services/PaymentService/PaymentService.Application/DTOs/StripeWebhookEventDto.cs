namespace PaymentService.Application.DTOs
{
    public class StripeWebhookEventDto
    {
        public string Type { get; set; } = null!;

        public string PaymentIntentId { get; set; } = null!;

        public string? FailureReason { get; set; }

        public string RefundId { get; set; } = string.Empty;

        public string? RefundStatus { get; set; }
    }
}
