using PaymentService.Domain.Enums;

namespace PaymentService.Application.DTOs
{
    public class RetryPaymentResultDto
    {
        public Guid PaymentId { get; set; }
        public string ClientSecret { get; set; } = null!;
        public PaymentStatus Status { get; set; }
    }
}
