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

        public string? StripePaymentIntentId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? CompletedAt { get; private set; }

        public DateTime? FailedAt { get; private set; }

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
            CompletedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed(string? reason = null)
        {
            Status = PaymentStatus.Failed;
            FailedAt = DateTime.UtcNow;
            FailureReason = reason;
        }

        public void MarkAsCancelled()
        {
            Status = PaymentStatus.Cancelled;
        }
    }
}
