using PaymentService.Domain.Enums;

namespace PaymentService.Application.DTOs
{
    public class CreatePaymentRequestDto
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public CurrencyCode Currency { get; set; }
    }
}
