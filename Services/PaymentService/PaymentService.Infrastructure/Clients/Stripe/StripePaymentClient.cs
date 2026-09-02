using Microsoft.Extensions.Configuration;
using PaymentService.Application.Abstractions;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Enums;
using Stripe;

namespace PaymentService.Infrastructure.Clients.Stripe
{
    public class StripePaymentClient(PaymentIntentService _paymentIntentService, RefundService _refundService, IConfiguration _configuration) : IStripePaymentClient
    {
        public async Task<StripePaymentResultDto> CreatePaymentIntentAsync(decimal amount, CurrencyCode currency, Guid paymentId, CancellationToken cancellationToken = default)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = currency.ToString().ToLowerInvariant(),
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    ["PaymentId"] = paymentId.ToString()
                }
            };

            var paymentIntent = await _paymentIntentService.CreateAsync(options,cancellationToken: cancellationToken);

            return new StripePaymentResultDto { PaymentIntentId = paymentIntent.Id, ClientSecret = paymentIntent.ClientSecret };
        }

        public async Task<string> CreateRefundAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {

            var options = new RefundCreateOptions { PaymentIntent = paymentIntentId };

            var refund = await _refundService.CreateAsync(options, cancellationToken: cancellationToken);

            return refund.Id;
        }

        public StripeWebhookEventDto ConstructWebhookEvent(string json, string stripeSignature)
        {
            var webhookSecret = _configuration["Stripe:WebhookSecret"] ?? throw new InvalidOperationException("Stripe webhook secret is not configured.");

            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            var refund = stripeEvent.Data.Object as Refund;

            return new StripeWebhookEventDto
            {
                EventId = stripeEvent.Id,
                Type = stripeEvent.Type,
                PaymentIntentId = paymentIntent?.Id ?? string.Empty,
                RefundId = refund?.Id ?? string.Empty,
                RefundStatus = refund?.Status,
                FailureReason = paymentIntent?.LastPaymentError?.Message ?? refund?.FailureReason
            };
        }
    }
}