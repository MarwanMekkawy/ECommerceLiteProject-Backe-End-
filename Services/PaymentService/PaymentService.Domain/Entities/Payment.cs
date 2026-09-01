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

        public int OrderCompletionReattempts { get; private set; } = 0;
        public DateTime? NextOrderCompletionAttemptAt { get; private set; }

        public string? StripePaymentIntentId { get; private set; }
        public string? StripeRefundId { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? SucceededAt { get; private set; }
        public DateTime? FailedAt { get; private set; }
        public DateTime? RefundedAt { get; private set; }

        public string? FailureReason { get; private set; }

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

        public void MarkAsFailed(string? reason = null)
        {
            Status = PaymentStatus.Failed;
            FailedAt = DateTime.UtcNow;
            FailureReason = reason;
        }

        // no bussiness reason yet for cancelling payment
        public void MarkAsCancelled()
        {
            Status = PaymentStatus.Cancelled;
        }

        public void MarkAsRefunding()
        {
            if (Status != PaymentStatus.Succeeded)
                throw new InvalidOperationException("Only a succeeded payment can be refunded.");
            Status = PaymentStatus.Refunding;
        }

        public void MarkAsRefunded(string stripeRefundId)
        {
            Status = PaymentStatus.Refunded;
            StripeRefundId = stripeRefundId;
            RefundedAt = DateTime.UtcNow;
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
    }
}
