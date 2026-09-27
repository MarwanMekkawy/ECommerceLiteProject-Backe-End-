using PaymentService.Application.Abstractions;
using PaymentService.Application.Abstractions.ClientsAbstractions;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Contracts;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Services
{
    public class PaymentAppService
        (IUnitOfWork _unitOfWork, IStripePaymentClient _stripePaymentClient, IOrderServiceClient _orderServiceClient, 
        INotificationServiceClient _notificationServiceClient) : IPaymentAppService
    {

        #region // Helper Methods ==============================================================================================================
        private async Task<Payment?> HandlePaymentSucceededAsync(string paymentIntentId, CancellationToken cancellationToken)
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

            return payment;
        }
        private async Task HandlePaymentFailedAndNotifyAsync(string paymentIntentId, string? failureReason, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripePaymentIntentIdAsync(paymentIntentId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Succeeded || payment.Status == PaymentStatus.RefundInitiated || payment.Status == PaymentStatus.RefundingByStripe ||
                payment.Status == PaymentStatus.Refunded || payment.Status == PaymentStatus.RefundFailed)
                return;

            payment.MarkAsFailed(failureReason);
            //notify failed
            await _notificationServiceClient.SendPaymentFailedAsync(payment.UserId, payment.CustomerEmail, "Customer", payment.OrderId, payment.Id, 
                                                                    payment.Amount.Amount, payment.Amount.Currency.ToString(), failureReason!, cancellationToken);
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
        private async Task HandleRefundCreatedAndNotifyAsync(string refundId, string? refundStatus, long? refundAmount, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripeRefundIdAsync(refundId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.Refunded || payment.Status == PaymentStatus.RefundFailed)
                return;

            if (payment.Status != PaymentStatus.RefundInitiated && payment.Status != PaymentStatus.RefundingByStripe)
                return;

            switch (refundStatus)
            {
                case "succeeded":
                    payment.MarkAsRefunded(refundId);
                    //notify refunded
                    await _notificationServiceClient.SendPaymentRefundedAsync(payment.UserId, payment.CustomerEmail, "Customer", payment.OrderId, 
                        payment.Id, refundAmount!.Value / 100m, cancellationToken);
                    break;

                case "pending":
                case "requires_action":
                    payment.MarkAsRefunding();
                    break;

                case "failed":
                    payment.MarkAsRefundFailedRequiresAdminAttention(refundId);
                    break;
            }
        }
        private async Task HandleRefundUpdatedAndNotifyAsync(string refundId, string? refundStatus, long? refundAmount, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripeRefundIdAsync(refundId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            switch (refundStatus)
            {
                case "succeeded":
                    if (payment.Status != PaymentStatus.Refunded)
                    {
                        payment.MarkAsRefunded(refundId);
                        //notify refunded if didnt already do it
                        await _notificationServiceClient.SendPaymentRefundedAsync(payment.UserId, payment.CustomerEmail, "Customer", payment.OrderId, 
                            payment.Id, refundAmount!.Value / 100m, cancellationToken);
                    }
                    break;

                case "pending":
                case "requires_action":
                    if (payment.Status != PaymentStatus.Refunded &&
                        payment.Status != PaymentStatus.RefundFailed)
                    {
                        payment.MarkAsRefunding();
                    }
                    break;

                case "failed":
                    payment.MarkAsRefundFailedRequiresAdminAttention(refundId);
                    break;
            }
        }
        private async Task HandleRefundFailedAsync(string refundId, string? failureReason, long? refundAmount, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByStripeRefundIdAsync(refundId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

            if (payment.Status == PaymentStatus.RefundFailed)
                return;

            payment.MarkAsRefundFailedRequiresAdminAttention(refundId, failureReason);
        }
        #endregion // ==========================================================================================================================

        public async Task<CreatePaymentResponseDto> CreatePaymentAsync(Guid orderId, Guid userId, decimal amount, CurrencyCode currency, string email, CancellationToken cancellationToken = default)
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

            var payment = new Payment(orderId, userId, new Money(amount, currency), email);

            var stripeResult = await _stripePaymentClient.CreatePaymentIntentAsync(amount, currency, payment.Id, cancellationToken);

            payment.SetStripePaymentIntentId(stripeResult.PaymentIntentId);

            await _unitOfWork.Payments.AddAsync(payment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreatePaymentResponseDto { PaymentId = payment.Id, ClientSecret = stripeResult.ClientSecret, Status = payment.Status };
        }

        public async Task RefundPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId, cancellationToken);

            if (payment is null)
                throw new InvalidOperationException("Payment not found.");

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

            Payment? paymentToComplete = null;

            switch (webhookEvent.Type)
            {
                case "payment_intent.succeeded":
                    paymentToComplete  = await HandlePaymentSucceededAsync(webhookEvent.PaymentIntentId, cancellationToken);
                    break;

                case "payment_intent.payment_failed":
                    await HandlePaymentFailedAndNotifyAsync(webhookEvent.PaymentIntentId, webhookEvent.FailureReason, cancellationToken);
                    break;

                case "payment_intent.processing":
                    await HandlePaymentProcessingAsync(webhookEvent.PaymentIntentId, cancellationToken);
                    break;

                case "payment_intent.requires_action":
                    await HandlePaymentRequiresActionAsync(webhookEvent.PaymentIntentId, cancellationToken);
                    break;

                // refunds
                case "refund.created":
                    await HandleRefundCreatedAndNotifyAsync(webhookEvent.RefundId, webhookEvent.RefundStatus, webhookEvent.RefundAmount, cancellationToken);
                    break;

                case "refund.updated":
                    await HandleRefundUpdatedAndNotifyAsync(webhookEvent.RefundId, webhookEvent.RefundStatus, webhookEvent.RefundAmount, cancellationToken);
                    break;

                case "refund.failed":
                    await HandleRefundFailedAsync(webhookEvent.RefundId, webhookEvent.FailureReason, webhookEvent.RefundAmount, cancellationToken);
                    break;
            }

            await _unitOfWork.ProcessedStripeEvents.AddAsync(new ProcessedStripeEvent(webhookEvent.EventId), cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (paymentToComplete is not null)
            {
                await _orderServiceClient.CompleteOrderAsync(paymentToComplete.OrderId , cancellationToken);
                paymentToComplete .MarkCompletionConfirmed();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
