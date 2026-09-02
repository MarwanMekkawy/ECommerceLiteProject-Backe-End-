using PaymentService.Application.Abstractions;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Contracts;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Services
{
    public class PaymentAppService(IUnitOfWork _unitOfWork, IStripePaymentClient _stripePaymentClient, IOrderServiceClient _orderServiceClient) : IPaymentAppService
    {

        #region // Helper Methods ==============================================================================================================
        private async Task<Guid?> HandlePaymentSucceededAsync(string paymentIntentId, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripePaymentIntentIdAsync(paymentIntentId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Succeeded && payment.IsOrderCompletionConfirmed)
                return null;

            if (payment.Status == PaymentStatus.RefundInitiated || payment.Status == PaymentStatus.RefundingByStripe || payment.Status == PaymentStatus.Refunded ||
                payment.Status == PaymentStatus.RefundFailed)
                return null;

            payment.MarkAsSucceeded();

            return payment.OrderId;
        }
        private async Task HandlePaymentFailedAsync(string paymentIntentId, string? failureReason, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripePaymentIntentIdAsync(paymentIntentId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Succeeded || payment.Status == PaymentStatus.RefundInitiated || payment.Status == PaymentStatus.RefundingByStripe ||
                payment.Status == PaymentStatus.Refunded || payment.Status == PaymentStatus.RefundFailed)
                return;

            payment.MarkAsFailed(failureReason);
        }
        private async Task HandlePaymentProcessingAsync(string paymentIntentId, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripePaymentIntentIdAsync(paymentIntentId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Succeeded || payment.Status == PaymentStatus.Failed || payment.Status == PaymentStatus.Cancelled || payment.Status == PaymentStatus.RefundInitiated ||
                payment.Status == PaymentStatus.RefundingByStripe || payment.Status == PaymentStatus.Refunded || payment.Status == PaymentStatus.RefundFailed)
                return;

            payment.MarkAsProcessing();
        }
        private async Task HandlePaymentRequiresActionAsync(string paymentIntentId, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripePaymentIntentIdAsync(paymentIntentId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Succeeded || payment.Status == PaymentStatus.Failed || payment.Status == PaymentStatus.Cancelled || payment.Status == PaymentStatus.RefundInitiated ||
                payment.Status == PaymentStatus.RefundingByStripe || payment.Status == PaymentStatus.Refunded || payment.Status == PaymentStatus.RefundFailed)
                return;

            payment.MarkAsRequiresAction();
        }
        // refund methods ===============================================================================================
        private async Task HandleRefundCreatedAsync(string refundId, string? refundStatus, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripeRefundIdAsync(refundId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Refunded || payment.Status == PaymentStatus.RefundFailed)
                return;

            if (payment.Status != PaymentStatus.RefundInitiated && payment.Status != PaymentStatus.RefundingByStripe)
                return;

            if (refundStatus == "succeeded")
                payment.MarkAsRefunded(refundId);
            else if (refundStatus == "failed")
                payment.MarkAsRefundFailedRequiresAdminAttention(refundId);
            else
                payment.MarkAsRefunding();
        }
        private async Task HandleRefundUpdatedAsync(string refundId, string? refundStatus, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripeRefundIdAsync(refundId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (refundStatus == "succeeded")
            {
                if (payment.Status == PaymentStatus.Refunded || payment.Status == PaymentStatus.RefundFailed)
                    return;

                payment.MarkAsRefunded(refundId);
            }
        }
        private async Task HandleRefundFailedAsync(string refundId, string? failureReason, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripeRefundIdAsync(refundId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Refunded || payment.Status == PaymentStatus.RefundFailed)
                return;

            payment.MarkAsRefundFailedRequiresAdminAttention(refundId, failureReason);
        }
        #endregion // ==========================================================================================================================

        public async Task<CreatePaymentResponseDto> CreatePaymentAsync(Guid orderId, Guid userId, decimal amount, CurrencyCode currency, CancellationToken cancellationToken = default)
        {
            var existingPayment = await _unitOfWork.Payments.GetByOrderIdAsync(orderId, cancellationToken);

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

            await _unitOfWork.Payments.AddAsync(payment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreatePaymentResponseDto { PaymentId = payment.Id, ClientSecret = stripeResult.ClientSecret, Status = payment.Status };
        }

        // {Not-Used} so far 
        public async Task RefundPaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            if (payment.Status == PaymentStatus.Refunded)
                return;

            if (payment.Status == PaymentStatus.RefundInitiated || payment.Status == PaymentStatus.RefundingByStripe)
                return;

            payment.MarkAsRefundInitiated();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var refundID = await _stripePaymentClient.CreateRefundAsync(payment.StripePaymentIntentId!, cancellationToken);

            payment.SetStripeRefundId(refundID);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task HandleStripeWebhookAsync(string json, string stripeSignature, CancellationToken cancellationToken = default)
        {
            var webhookEvent = _stripePaymentClient.ConstructWebhookEvent(json, stripeSignature);

            var alreadyProcessed = await _unitOfWork.ProcessedStripeEvents.ExistsAsync(webhookEvent.EventId, cancellationToken);

            if (alreadyProcessed)
                return;

            Guid? orderIdToComplete = null;

            switch (webhookEvent.Type)
            {
                case "payment_intent.succeeded":
                    orderIdToComplete = await HandlePaymentSucceededAsync(webhookEvent.PaymentIntentId, cancellationToken);
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

                // refunds
                case "refund.created":
                    await HandleRefundCreatedAsync(webhookEvent.RefundId, webhookEvent.RefundStatus, cancellationToken);
                    break;

                case "refund.updated":
                    await HandleRefundUpdatedAsync(webhookEvent.RefundId, webhookEvent.RefundStatus, cancellationToken);
                    break;

                case "refund.failed":
                    await HandleRefundFailedAsync(webhookEvent.RefundId, webhookEvent.FailureReason, cancellationToken);
                    break;
            }

            await _unitOfWork.ProcessedStripeEvents.AddAsync(new ProcessedStripeEvent(webhookEvent.EventId), cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (orderIdToComplete.HasValue)
            {
                await _orderServiceClient.CompleteOrderAsync(orderIdToComplete.Value, cancellationToken);
            }
        }
    }
}
