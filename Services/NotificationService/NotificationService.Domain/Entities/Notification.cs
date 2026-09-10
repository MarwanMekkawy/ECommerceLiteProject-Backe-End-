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
        public string? Data { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public int AttemptCount { get; private set; }
        public DateTime? NextAttemptAt { get; private set; }
        public DateTime? SentAt { get; private set; }
        public string? FailureReason { get; private set; }

        private Notification() { }

        public Notification(Guid userId, EmailAddress recipientEmail, NotificationType type, string subject, string? data)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            RecipientEmail = recipientEmail;
            Type = type;
            Subject = subject;
            Status = NotificationStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            Data = data;
        }

        public void AttemptToSend()
        {
            if (Status == NotificationStatus.Sent)
                throw new InvalidOperationException("Notification has already been sent.");

            if (AttemptCount >= 5)
                throw new InvalidOperationException("Maximum attempts reached.");

            AttemptCount++;

            NextAttemptAt = AttemptCount switch
            {
                1 => DateTime.UtcNow,
                2 => DateTime.UtcNow.AddMinutes(1),
                3 => DateTime.UtcNow.AddMinutes(5),
                4 => DateTime.UtcNow.AddMinutes(15),
                5 => DateTime.UtcNow.AddMinutes(30),
                _ => throw new InvalidOperationException()
            };
        }

        public void MarkAsSent()
        {
            if (Status == NotificationStatus.Sent) return;

            Status = NotificationStatus.Sent;
            SentAt = DateTime.UtcNow;
            NextAttemptAt = null;
            FailureReason = null;
        }

        public void MarkAsFailed(string reason)
        {
            if (Status == NotificationStatus.Sent) return;

            Status = NotificationStatus.Failed;
            FailureReason = reason;
        }
    }
}
