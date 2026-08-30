using PaymentService.Application.Abstractions;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Contracts;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Services
{
    public class PaymentAppService(IPaymentRepository _paymentRepository, IStripePaymentClient _stripePaymentClient) : IPaymentAppService
    {

        #region // Helper Methods ==============================================================================================================
        private async Task HandlePaymentSucceededAsync(string paymentIntentId, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByStripePaymentIntentIdAsync(paymentIntentId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Succeeded)
                return;

            payment.MarkAsSucceeded();

            await _paymentRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task HandlePaymentFailedAsync(string paymentIntentId, string? failureReason, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByStripePaymentIntentIdAsync(paymentIntentId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Succeeded)
                return;

            payment.MarkAsFailed(failureReason);

            await _paymentRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task HandlePaymentProcessingAsync(string paymentIntentId, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByStripePaymentIntentIdAsync(paymentIntentId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Succeeded || payment.Status == PaymentStatus.Failed || payment.Status == PaymentStatus.Cancelled)
                return;

            payment.MarkAsProcessing();

            await _paymentRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task HandlePaymentRequiresActionAsync(string paymentIntentId, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByStripePaymentIntentIdAsync(paymentIntentId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Succeeded || payment.Status == PaymentStatus.Failed || payment.Status == PaymentStatus.Cancelled)
                return;

            payment.MarkAsRequiresAction();

            await _paymentRepository.SaveChangesAsync(cancellationToken);
        }
        #endregion // ==========================================================================================================================

        public async Task<CreatePaymentResponseDto> CreatePaymentAsync(Guid orderId, Guid userId, decimal amount, CurrencyCode currency, CancellationToken cancellationToken = default)
        {
            var existingPayment = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);

            if (existingPayment is not null)
            {
                if (existingPayment.Status == PaymentStatus.Succeeded)
                {
                    return new CreatePaymentResponseDto
                    {
                        PaymentId = existingPayment.Id,
                        Status = existingPayment.Status
                    };
                }

                throw new InvalidOperationException("A payment already exists for this order.");
            }

            var payment = new Payment(orderId, userId, new Money(amount, currency));

            var stripeResult = await _stripePaymentClient.CreatePaymentIntentAsync(amount, currency, payment.Id, cancellationToken);

            payment.SetStripePaymentIntentId(stripeResult.PaymentIntentId);

            await _paymentRepository.AddAsync(payment, cancellationToken);

            await _paymentRepository.SaveChangesAsync(cancellationToken);

            return new CreatePaymentResponseDto { PaymentId = payment.Id, ClientSecret = stripeResult.ClientSecret, Status = payment.Status };
        }

        public async Task HandleStripeWebhookAsync(string json, string stripeSignature, CancellationToken cancellationToken = default)
        {
            var webhookEvent = _stripePaymentClient.ConstructWebhookEvent(json, stripeSignature);

            switch (webhookEvent.Type)
            {
                case "payment_intent.succeeded":
                    await HandlePaymentSucceededAsync(webhookEvent.PaymentIntentId, cancellationToken);
                    break;

                case "payment_intent.payment_failed":
                    await HandlePaymentFailedAsync(webhookEvent.PaymentIntentId, webhookEvent.FailureReason, cancellationToken);
                    break;

                case "payment_intent.processing":
                    await HandlePaymentProcessingAsync(webhookEvent.PaymentIntentId, cancellationToken);
                    break;

                case "payment_intent.requires_action":
                    await HandlePaymentRequiresActionAsync(webhookEvent.PaymentIntentId, cancellationToken);
                    break;
            }
        }
    }
}