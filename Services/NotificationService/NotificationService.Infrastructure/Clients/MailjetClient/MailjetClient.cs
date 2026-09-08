using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using IMailjetClient = NotificationService.Application.Abstractions.IMailjetClient;

namespace NotificationService.Infrastructure.Clients.MailjetClient
{
    public class MailjetClient(IConfiguration config) : IMailjetClient
    {
        private readonly Mailjet.Client.MailjetClient _client =new(config["Mailjet:ApiKey"]!, config["Mailjet:SecretKey"]!);

        public async Task SendTemplateAsync(string recipientEmail, string? recipientName, int templateId, Guid notificationId, IReadOnlyDictionary<string, object>? variables)
        {
            var message = new JObject
            {
                ["From"] = new JObject
                {
                    ["Email"] = config["Mailjet:FromEmail"]!,
                    ["Name"] = config["Mailjet:FromName"]!
                },
                ["To"] = new JArray
                {
                    new JObject
                    {
                        ["Email"] = recipientEmail,
                        ["Name"] = recipientName
                    }
                },
                ["TemplateID"] = templateId,
                ["TemplateLanguage"] = true,
                ["CustomID"] = notificationId.ToString()
            };

            if (variables is not null)
            {
                message["Variables"] = JObject.FromObject(variables);
            }

            var request = new MailjetRequest { Resource = SendV31.Resource };

            request.Property(Send.Messages, new JArray { message });

            var response = await _client.PostAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Mailjet request failed. " + $"Status: {response.StatusCode}. " + $"Error: {response.GetErrorInfo()}");
            }
        }
    }
}
