namespace PaymentService.Application.DTOs
{
    public class StripeWebhookEventDto
    {
        public string Type { get; set; } = null!;

        public string PaymentIntentId { get; set; } = null!;

        public string? FailureReason { get; set; }
    }
}
