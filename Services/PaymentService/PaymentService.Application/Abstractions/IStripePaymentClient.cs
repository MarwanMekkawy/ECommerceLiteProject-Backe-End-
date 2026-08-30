using PaymentService.Application.DTOs;
using PaymentService.Domain.Enums;

namespace PaymentService.Application.Abstractions
{
    public interface IStripePaymentClient
    {
        Task<StripePaymentResultDto> CreatePaymentIntentAsync(decimal amount, CurrencyCode currency, Guid paymentId, CancellationToken cancellationToken = default);
        StripeWebhookEventDto ConstructWebhookEvent(string json, string stripeSignature);
    }
}