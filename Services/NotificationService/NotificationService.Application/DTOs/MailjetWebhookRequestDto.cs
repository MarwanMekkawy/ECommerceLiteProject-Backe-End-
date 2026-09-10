namespace NotificationService.Application.DTOs
{
    public class MailjetWebhookRequest
    {
        public string Event { get; set; } = null!;
        public long Time { get; set; }
        public string Email { get; set; } = null!;
        public long MessageID { get; set; }
        public string? CustomID { get; set; }
        public string? Payload { get; set; }

        public bool? Blocked { get; set; }
        public bool? HardBounce { get; set; }

        public string? ErrorRelatedTo { get; set; }
        public string? Error { get; set; }
    }
}
