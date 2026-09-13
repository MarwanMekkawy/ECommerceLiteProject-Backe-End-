using PaymentService.Application.Abstractions.ClientsAbstractions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PaymentService.Infrastructure.Clients.NotificationServiceClient
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


        public async Task SendPaymentFailedAsync
            (Guid userId, string recipientEmail, string firstName, Guid orderId, Guid paymentId, decimal amount, string currency, string failureReason, CancellationToken cancellationToken)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new SendEmailNotificationRequestDto
            {
                UserId = userId,
                RecipientEmail = recipientEmail,
                Type = NotificationType.PaymentFailed,
                Data = new Dictionary<string, object>
                {
                    ["firstName"] = firstName,
                    ["orderId"] = orderId,
                    ["paymentId"] = paymentId,
                    ["amount"] = amount,
                    ["currency"] = currency,
                    ["failureReason"] = failureReason
                }
            };

            await SendAsync(request, token);
        }

        public async Task SendPaymentRefundedAsync
            (Guid userId, string recipientEmail, string firstName, Guid orderId, Guid paymentId, decimal refundAmount, CancellationToken cancellationToken)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new SendEmailNotificationRequestDto
            {
                UserId = userId,
                RecipientEmail = recipientEmail,
                Type = NotificationType.PaymentRefunded,
                Data = new Dictionary<string, object>
                {
                    ["firstName"] = firstName,
                    ["orderId"] = orderId,
                    ["paymentId"] = paymentId,
                    ["refundAmount"] = refundAmount
                }
            };

            await SendAsync(request, token);
        }
    }
}
