using NotificationService.Domain.Enums;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public EmailAddress RecipientEmail { get; private set; } = null!;
        public NotificationType Type { get; private set; }
        public NotificationStatus Status { get; private set; }
        public string Subject { get; private set; } = null!;

        public int AttemptCount { get; private set; }
        public DateTime? LastAttemptAt { get; private set; }
        public DateTime? SentAt { get; private set; }
        public string? FailureReason { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Notification() { }

        public Notification(Guid userId, EmailAddress recipientEmail, NotificationType type, string subject)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            RecipientEmail = recipientEmail;
            Type = type;
            Subject = subject;
            Status = NotificationStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void RecordAttempt()
        {
            AttemptCount++;
            LastAttemptAt = DateTime.UtcNow;
        }

        public void MarkAsSent()
        {
            Status = NotificationStatus.Sent;
            SentAt = DateTime.UtcNow;
            FailureReason = null;
        }

        public void MarkAsFailed(string reason)
        {
            Status = NotificationStatus.Failed;
            FailureReason = reason;
        }
    }
}
