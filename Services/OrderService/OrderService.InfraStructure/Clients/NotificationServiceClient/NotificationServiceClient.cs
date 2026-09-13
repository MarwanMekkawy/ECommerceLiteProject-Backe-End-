using OrderService.Application.Abstractions.ClientsAbstractions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.InfraStructure.Clients.NotificationServiceClient
{
    public class NotificationServiceClient(HttpClient httpClient, IServiceTokenClient serviceTokenClient) : INotificationServiceClient
    {
        private async Task SendAsync(SendEmailNotificationRequestDto request, string token)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "notifications");

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            httpRequest.Content = JsonContent.Create(request);

            using var response = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                string? error = null;
                try
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var document = JsonDocument.Parse(json);
                    error = document.RootElement.GetProperty("error").GetString();
                }
                catch
                {
                }

                throw new Exception(error ?? $"NotificationService returned {(int)response.StatusCode} {response.StatusCode}");
            }
        }

        public async Task SendOrderConfirmedAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, decimal total, string currency, DateTime paymentExpiresAt, CancellationToken cancellationToken)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new SendEmailNotificationRequestDto
            {
                UserId = userId,
                RecipientEmail = recipientEmail,
                Type = NotificationType.OrderConfirmed,
                Data = new Dictionary<string, object>
                {
                    ["firstName"] = firstName,
                    ["orderId"] = orderId,
                    ["total"] = total,
                    ["currency"] = currency,
                    ["paymentExpiresAt"] = paymentExpiresAt
                }
            };

            await SendAsync(request, token);
        }

        public async Task SendOrderCompletedAsync
            (Guid userId, string recipientEmail, string firstName, Guid orderId, object items, decimal total, string currency, CancellationToken cancellationToken)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new SendEmailNotificationRequestDto
            {
                UserId = userId,
                RecipientEmail = recipientEmail,
                Type = NotificationType.OrderCompleted,
                Data = new Dictionary<string, object>
                {
                    ["firstName"] = firstName,
                    ["orderId"] = orderId,
                    ["items"] = items,
                    ["total"] = total,
                    ["currency"] = currency
                }
            };

            await SendAsync(request, token);
        }

        public async Task SendOrderCancelledAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, string reason, CancellationToken cancellationToken)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new SendEmailNotificationRequestDto
            {
                UserId = userId,
                RecipientEmail = recipientEmail,
                Type = NotificationType.OrderCancelled,
                Data = new Dictionary<string, object>
                {
                    ["firstName"] = firstName,
                    ["orderId"] = orderId,
                    ["reason"] = reason
                }
            };

            await SendAsync(request, token);
        }

        public async Task SendOrderExpiredAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, CancellationToken cancellationToken)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new SendEmailNotificationRequestDto
            {
                UserId = userId,
                RecipientEmail = recipientEmail,
                Type = NotificationType.OrderExpired,
                Data = new Dictionary<string, object>
                {
                    ["firstName"] = firstName,
                    ["orderId"] = orderId
                }
            };

            await SendAsync(request, token);
        }
    }
}
