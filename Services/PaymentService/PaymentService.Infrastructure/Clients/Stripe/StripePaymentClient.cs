using Microsoft.Extensions.Configuration;
using PaymentService.Application.Abstractions;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Enums;
using Stripe;

namespace PaymentService.Infrastructure.Clients.Stripe
{
    public class StripePaymentClient(PaymentIntentService _paymentIntentService, IConfiguration _configuration) : IStripePaymentClient
    {
        public async Task<StripePaymentResultDto> CreatePaymentIntentAsync(decimal amount, CurrencyCode currency, Guid paymentId, CancellationToken cancellationToken = default)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = currency.ToString().ToLowerInvariant(),
                Metadata = new Dictionary<string, string>
                {
                    ["PaymentId"] = paymentId.ToString()
                }
            };

            var paymentIntent = await _paymentIntentService.CreateAsync(options,cancellationToken: cancellationToken);

            return new StripePaymentResultDto { PaymentIntentId = paymentIntent.Id, ClientSecret = paymentIntent.ClientSecret };
        }

        public StripeWebhookEventDto ConstructWebhookEvent(string json, string stripeSignature)
        {
            var webhookSecret = _configuration["Stripe:WebhookSecret"] ?? throw new InvalidOperationException("Stripe webhook secret is not configured.");

            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

            return new StripeWebhookEventDto
            {
                Type = stripeEvent.Type,
                PaymentIntentId = paymentIntent?.Id ?? string.Empty,
                FailureReason = paymentIntent?.LastPaymentError?.Message
            };
        }
    }
}