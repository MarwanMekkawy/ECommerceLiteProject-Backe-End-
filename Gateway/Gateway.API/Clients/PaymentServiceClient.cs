using Gateway.API.Abstractions;
using Gateway.API.DTOs;
using Gateway.API.Exceptions;

namespace Gateway.API.Clients
{
    public class PaymentServiceClient(HttpClient httpClient, HttpContextAccessor httpContextAccessor) : IPaymentServiceClient
    {
        #region cosntructing the requests[helpers]==================================================================================================================
        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, object? request = null, CancellationToken cancellationToken = default)
        {
            using var message = new HttpRequestMessage(method, endpoint);

            if (request is not null)
                message.Content = JsonContent.Create(request);

            var context = httpContextAccessor.HttpContext;

            if (context is not null)
            {
                if (context.Request.Headers.TryGetValue("Authorization", out var authorization))
                    message.Headers.TryAddWithoutValidation("Authorization", authorization.ToString());

                if (context.Request.Headers.TryGetValue("Cookie", out var cookie))
                    message.Headers.TryAddWithoutValidation("Cookie", cookie.ToString());
            }

            var response = await httpClient.SendAsync(message, cancellationToken);

            if (context is not null && response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                    context.Response.Headers.Append("Set-Cookie", cookie);
            }

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                response.Dispose();
                throw new DownstreamServiceException(statusCode, responseBody);
            }

            return response;
        }

        private async Task SendNoContentAsync(HttpMethod method, string endpoint, object? request = null, CancellationToken cancellationToken = default)
        {
            using var response = await SendAsync(method, endpoint, request, cancellationToken);
        }

        private async Task<T> SendForResultAsync<T>(HttpMethod method, string endpoint, object? request = null, CancellationToken cancellationToken = default)
        {
            using var response = await SendAsync(method, endpoint, request, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);

            return result ?? throw new InvalidOperationException($"PaymentService returned an empty response for {endpoint}.");
        }
        #endregion =================================================================================================================================================

        public async Task<RetryPaymentResponseDto> RetryPaymentAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await SendForResultAsync<RetryPaymentResponseDto>(HttpMethod.Post, $"payments/orders/{orderId}/retry", cancellationToken: cancellationToken);
        }

        public async Task RefundPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, $"payments/admin/{paymentId}/refund", cancellationToken: cancellationToken);
        }
    }
}
