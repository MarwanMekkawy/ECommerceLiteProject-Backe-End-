using PaymentService.Application.DTOs;
using PaymentService.Domain.Enums;

namespace PaymentService.Application.Abstractions
{
    public interface IPaymentAppService
    {
        Task<CreatePaymentResponseDto> CreatePaymentAsync(Guid orderId, Guid userId, decimal amount, CurrencyCode currency, CancellationToken cancellationToken = default);
        Task HandleStripeWebhookAsync(string json, string stripeSignature, CancellationToken cancellationToken = default);
    }
}