using OrderService.Application.Abstractions;
using OrderService.Domain.Enums;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.InfraStructure.Clients.PaymentServiceClient
{
    public class PaymentServiceClient(HttpClient httpClient, IServiceTokenClient serviceTokenClient) : IPaymentServiceClient
    {
        public async Task<(Guid PaymentId, string ClientSecret, string Status)> CreatePaymentAsync(Guid orderId, Guid userId, decimal amount, CurrencyCode currency, CancellationToken cancellationToken)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            using var request = new HttpRequestMessage(HttpMethod.Post, "payments/create-internal");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var paymentRequest = new CreatePaymentRequestDto { OrderId = orderId, UserId = userId, Amount = amount, Currency = currency };

            request.Content = JsonContent.Create(paymentRequest);

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

                throw new Exception(
                    error ?? $"PaymentService returned {(int)response.StatusCode} {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            using var documentResponse = JsonDocument.Parse(jsonResponse);

            var paymentId = documentResponse.RootElement.GetProperty("paymentId").GetGuid();
            var clientSecret = documentResponse.RootElement.GetProperty("clientSecret").GetString()!;
            var status = documentResponse.RootElement.GetProperty("status").GetString()!;

            return (paymentId, clientSecret, status);
        }
    }
}
