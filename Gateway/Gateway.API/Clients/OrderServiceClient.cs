using Gateway.API.Abstractions;
using Gateway.API.DTOs;
using Gateway.API.Exceptions;

namespace Gateway.API.Clients
{
    public class OrderServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : IOrderServiceClient
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

            return result ?? throw new InvalidOperationException($"OrderService returned an empty response for {endpoint}.");
        }

        private async Task<T?> SendForNullableResultAsync<T>(HttpMethod method, string endpoint, object? request = null, CancellationToken cancellationToken = default)
        {
            using var response = await SendAsync(method, endpoint, request, cancellationToken);

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
        #endregion =================================================================================================================================================

        public async Task<IReadOnlyList<OrderResponseDto>> GetCurrentUserOrdersAsync(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
        {
            return await SendForResultAsync<List<OrderResponseDto>>(
                HttpMethod.Get, $"orders?pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken: cancellationToken);
        }

        public async Task<OrderResponseDto?> GetLatestOrderAsync(CancellationToken cancellationToken = default)
        {
            return await SendForNullableResultAsync<OrderResponseDto>(
                HttpMethod.Get, "orders/latest", cancellationToken: cancellationToken);
        }

        public async Task<OrderResponseDto?> GetCurrentUserOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await SendForNullableResultAsync<OrderResponseDto>(
                HttpMethod.Get, $"orders/{orderId}", cancellationToken: cancellationToken);
        }

        public async Task CreateUserOrderAsync(List<CreateOrderItemDto> items, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, "orders", items, cancellationToken);
        }

        public async Task AddItemToCartAsync(CreateOrderItemDto request, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, "orders/item", request, cancellationToken);
        }

        public async Task IncreaseItemQuantityAsync(Guid orderId, Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, $"orders/{orderId}/items/{productId}/increase", quantity, cancellationToken);
        }

        public async Task DecreaseItemQuantityAsync(Guid orderId, Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, $"orders/{orderId}/items/{productId}/decrease", quantity, cancellationToken);
        }

        public async Task<CheckoutOrderDto> CheckoutAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await SendForResultAsync<CheckoutOrderDto>(
                HttpMethod.Post, $"orders/{orderId}/checkout", cancellationToken: cancellationToken);
        }

        public async Task CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, $"orders/{orderId}/cancel", cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<OrderResponseDto>> GetAllOrdersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await SendForResultAsync<List<OrderResponseDto>>(
                HttpMethod.Get, $"orders/admin?pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken: cancellationToken);
        }

        public async Task<OrderResponseDto?> GetOrderByIdForAdminAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await SendForNullableResultAsync<OrderResponseDto>(
                HttpMethod.Get, $"orders/admin/{orderId}", cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<OrderResponseDto>> GetUserOrdersForAdminAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await SendForResultAsync<List<OrderResponseDto>>(
                HttpMethod.Get, $"orders/admin/user/{userId}?pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken: cancellationToken);
        }
    }
}
