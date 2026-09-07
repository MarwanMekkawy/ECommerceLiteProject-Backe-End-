using OrderService.Domain.Enums;

namespace OrderService.Application.Abstractions
{
    public interface IPaymentServiceClient
    {
       Task<(Guid PaymentId, string ClientSecret, string Status)> CreatePaymentAsync(Guid orderId, Guid userId, decimal amount, CurrencyCode currency, CancellationToken cancellationToken);
    }
}
