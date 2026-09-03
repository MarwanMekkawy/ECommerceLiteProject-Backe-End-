using OrderService.Domain.Enums;

namespace OrderService.InfraStructure.Clients.PaymentServiceClient
{
    public class CreatePaymentRequestDto
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public CurrencyCode Currency { get; set; }
    }
}
