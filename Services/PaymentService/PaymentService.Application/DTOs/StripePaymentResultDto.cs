namespace PaymentService.Application.DTOs
{
    public class StripePaymentResultDto
    {
        public string PaymentIntentId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }
}
