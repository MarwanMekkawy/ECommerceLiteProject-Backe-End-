using PaymentService.Application.Abstractions;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PaymentService.Infrastructure.Clients
{
    public class OrderServiceClient(HttpClient httpClient, IServiceTokenClient serviceTokenClient) : IOrderServiceClient
    {
        public async Task CompleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"orders/{orderId}/complete-internal");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                string? error = null;
                try
                {
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);
                    using var document = JsonDocument.Parse(json);
                    error = document.RootElement.GetProperty("error").GetString();
                }
                catch
                {
                }
                throw new Exception(error ?? $"OrderService returned {(int)response.StatusCode} {response.StatusCode}");
            }
        }
        public async Task CancelRefundedOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"orders/{orderId}/cancel-internal");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                string? error = null;
                try
                {
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);
                    using var document = JsonDocument.Parse(json);
                    error = document.RootElement.GetProperty("error").GetString();
                }
                catch
                {
                }
                throw new Exception(error ?? $"OrderService returned {(int)response.StatusCode} {response.StatusCode}");
            }
        }
    }
}
