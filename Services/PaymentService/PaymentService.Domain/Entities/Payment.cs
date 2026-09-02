using PaymentService.Domain.Enums;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }
        public Money Amount { get; private set; } = null!;
        public PaymentStatus Status { get; private set; }

        public bool IsOrderCompletionConfirmed { get; private set; }
        public bool IsOrderCancellationDueToRefundConfirmed { get; private set; }


        public int OrderCompletionReattempts { get; private set; } = 0;
        public DateTime? NextOrderCompletionAttemptAt { get; private set; }

        public int OrderCancellationReattempts { get; private set; } = 0;
        public DateTime? NextOrderCancellationAttemptAt { get; private set; }

        public string? StripePaymentIntentId { get; private set; }
        public string? StripeRefundId { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? SucceededAt { get; private set; }
        public DateTime? PaymentFailedAt { get; private set; }
        public DateTime? RefundInitiatedAt  { get; private set; }
        public DateTime? RefundedAt { get; private set; }
        public DateTime? RefundFailedAt { get; private set; }

        public string? PaymentFailureReason { get; private set; }
        public string? RefundFailureReason { get; private set; }

        private Payment() { }

        public Payment(Guid orderId, Guid userId, Money amount)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetStripePaymentIntentId(string stripePaymentIntentId)
        {
            StripePaymentIntentId = stripePaymentIntentId;
        }

        public void SetStripeRefundId(string stripeRefundId)
        {
            StripeRefundId = stripeRefundId;
        }

        public void MarkAsRequiresAction()
        {
            Status = PaymentStatus.RequiresAction;
        }

        public void MarkAsProcessing()
        {
            Status = PaymentStatus.Processing;
        }

        public void MarkAsSucceeded()
        {
            Status = PaymentStatus.Succeeded;
            SucceededAt = DateTime.UtcNow;
        }

        public void MarkCompletionConfirmed()
        {
            IsOrderCompletionConfirmed = true;
        }

        public void MarkCancellationDueToRefundConfirmed()
        {
            IsOrderCancellationDueToRefundConfirmed = true;
        }       

        public void MarkAsFailed(string? reason = null)
        {
            Status = PaymentStatus.Failed;
            PaymentFailedAt = DateTime.UtcNow;
            PaymentFailureReason = reason;
        }

        // no bussiness reason yet for cancelling payment
        public void MarkAsCancelled()
        {
            Status = PaymentStatus.Cancelled;
        }

        public void MarkAsRefundInitiated()
        {
            if (Status != PaymentStatus.Succeeded)
                throw new InvalidOperationException("Only a succeeded payment can initiate a refund.");

            Status = PaymentStatus.RefundInitiated;
            RefundInitiatedAt  = DateTime.UtcNow;
        }

        public void MarkAsRefunding()
        {
            if (Status != PaymentStatus.RefundInitiated)
                throw new InvalidOperationException("Only an initiated refund can become refunding.");

            Status = PaymentStatus.RefundingByStripe;
        }      

        public void MarkAsRefunded(string stripeRefundId)
        {
            Status = PaymentStatus.Refunded;
            StripeRefundId = stripeRefundId;
            RefundedAt = DateTime.UtcNow;
        }

        public void MarkAsRefundFailedRequiresAdminAttention(string stripeRefundId, string? reason = null)
        {
            Status = PaymentStatus.RefundFailed;
            StripeRefundId = stripeRefundId;
            RefundFailedAt = DateTime.UtcNow;
            RefundFailureReason = reason;
        }

        public void AttemptToComplete()
        {
            if(OrderCompletionReattempts >= 5) throw new InvalidOperationException("Maximum attempts reached.");
            OrderCompletionReattempts++;
            NextOrderCompletionAttemptAt = OrderCompletionReattempts switch
            {
                1 => DateTime.UtcNow,
                2 => DateTime.UtcNow.AddMinutes(1),
                3 => DateTime.UtcNow.AddMinutes(5),
                4 => DateTime.UtcNow.AddMinutes(15),
                5 => DateTime.UtcNow.AddMinutes(30),
                _ => throw new InvalidOperationException()
            };
        }

        public void AttemptToCancelRefundedOrder()
        {
            if (OrderCancellationReattempts >= 5) throw new InvalidOperationException("Maximum attempts reached.");
            OrderCancellationReattempts++;
            NextOrderCancellationAttemptAt = OrderCancellationReattempts switch
            {
                1 => DateTime.UtcNow,
                2 => DateTime.UtcNow.AddMinutes(1),
                3 => DateTime.UtcNow.AddMinutes(5),
                4 => DateTime.UtcNow.AddMinutes(15),
                5 => DateTime.UtcNow.AddMinutes(30),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
