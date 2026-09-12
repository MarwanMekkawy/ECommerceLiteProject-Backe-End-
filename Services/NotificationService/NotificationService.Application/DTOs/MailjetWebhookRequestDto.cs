using System.Text.Json.Serialization;

namespace NotificationService.Application.DTOs
{
    public class MailjetWebhookRequest
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = null!;

        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("MessageID")]
        public long MessageID { get; set; }

        [JsonPropertyName("CustomID")]
        public string? CustomID { get; set; }

        [JsonPropertyName("Payload")]
        public string? Payload { get; set; }

        [JsonPropertyName("blocked")]
        public bool? Blocked { get; set; }

        [JsonPropertyName("hard_bounce")]
        public bool? HardBounce { get; set; }

        [JsonPropertyName("error_related_to")]
        public string? ErrorRelatedTo { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
