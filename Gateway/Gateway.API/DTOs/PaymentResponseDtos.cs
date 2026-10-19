namespace Gateway.API.DTOs
{
    public class RetryPaymentResponseDto
    {
        public Guid PaymentId { get; set; }
        public string ClientSecret { get; set; } = null!;
        public string Status { get; set; } = string.Empty;
    }    
}
