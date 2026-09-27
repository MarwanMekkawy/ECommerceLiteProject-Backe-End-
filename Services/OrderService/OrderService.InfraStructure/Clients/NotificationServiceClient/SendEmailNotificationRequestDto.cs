using System.Text.Json.Serialization;

namespace OrderService.InfraStructure.Clients.NotificationServiceClient
{
    public class SendEmailNotificationRequestDto
    {
        public Guid UserId { get; set; }
        public string RecipientEmail { get; set; } = null!;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationType Type { get; set; }
        public Dictionary<string, object> Data { get; set; } = [];
    }
}