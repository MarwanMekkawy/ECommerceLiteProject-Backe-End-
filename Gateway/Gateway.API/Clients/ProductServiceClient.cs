using Gateway.API.Abstractions;
using Gateway.API.DTOs;
using Gateway.API.Exceptions;

namespace Gateway.API.Clients
{
    public class ProductServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : IProductServiceClient
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
            return result ?? throw new InvalidOperationException($"ProductService returned an empty response for {endpoint}.");
        }

        private async Task<T?> SendForNullableResultAsync<T>(HttpMethod method, string endpoint, object? request = null, CancellationToken cancellationToken = default)
        {
            using var response = await SendAsync(method, endpoint, request, cancellationToken);
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }

        #endregion =================================================================================================================================================

        // Public categories
        public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await SendForResultAsync<IReadOnlyList<CategoryDto>>(HttpMethod.Get, $"categories?pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken: cancellationToken);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await SendForNullableResultAsync<CategoryDto>(HttpMethod.Get, $"categories/{id}", cancellationToken: cancellationToken);
        }

        public async Task<CategoryDto?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await SendForNullableResultAsync<CategoryDto>(HttpMethod.Get, $"categories/by-name?name={Uri.EscapeDataString(name)}", cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<CategoryDto>> SearchCategoriesAsync(string name, CancellationToken cancellationToken)
        {
            return await SendForResultAsync<IReadOnlyList<CategoryDto>>(HttpMethod.Get, $"categories/search?name={Uri.EscapeDataString(name)}", cancellationToken: cancellationToken);
        }

        // Admin categories
        public async Task<IReadOnlyList<CategoryDto>> GetAllCategoriesForAdminAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await SendForResultAsync<IReadOnlyList<CategoryDto>>(HttpMethod.Get, $"categories/admin?pageNumber={pageNumber}&pageSize={pageSize}", cancellationToken: cancellationToken);
        }

        public async Task<CategoryDto?> GetCategoryByIdForAdminAsync(Guid id, CancellationToken cancellationToken)
        {
            return await SendForNullableResultAsync<CategoryDto>(HttpMethod.Get, $"categories/admin/{id}", cancellationToken: cancellationToken);
        }

        public async Task<CategoryDto?> GetCategoryByNameForAdminAsync(string name, CancellationToken cancellationToken)
        {
            return await SendForNullableResultAsync<CategoryDto>(HttpMethod.Get, $"categories/admin/by-name?name={Uri.EscapeDataString(name)}", cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<CategoryDto>> SearchCategoriesForAdminAsync(string name, CancellationToken cancellationToken)
        {
            return await SendForResultAsync<IReadOnlyList<CategoryDto>>(HttpMethod.Get, $"categories/admin/search?name={Uri.EscapeDataString(name)}", cancellationToken: cancellationToken);
        }

        public async Task<Guid> CreateCategoryAsync(CreateCategoryRequestDto dto, CancellationToken cancellationToken)
        {
            return await SendForResultAsync<Guid>(HttpMethod.Post, "categories", dto, cancellationToken);
        }

        public async Task UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto dto, CancellationToken cancellationToken)
        {
            await SendNoContentAsync(HttpMethod.Put, $"categories/{id}", dto, cancellationToken);
        }

        public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken)
        {
            await SendNoContentAsync(HttpMethod.Delete, $"categories/{id}", cancellationToken: cancellationToken);
        }

        public async Task ActivateCategoryAsync(Guid id, CancellationToken cancellationToken)
        {
            await SendNoContentAsync(HttpMethod.Patch, $"categories/{id}/activate", cancellationToken: cancellationToken);
        }

        public async Task DeactivateCategoryAsync(Guid id, CancellationToken cancellationToken)
        {
            await SendNoContentAsync(HttpMethod.Patch, $"categories/{id}/deactivate", cancellationToken: cancellationToken);
        }

        // Public products

        public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(int pageNumber, int pageSize, Guid? categoryId, CancellationToken cancellationToken)
        {
            var endpoint = $"products?pageNumber={pageNumber}&pageSize={pageSize}";
            if (categoryId.HasValue)
                endpoint += $"&categoryId={categoryId.Value}";

            return await SendForResultAsync<IReadOnlyList<ProductDto>>(HttpMethod.Get, endpoint, cancellationToken: cancellationToken);
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await SendForNullableResultAsync<ProductDto>(HttpMethod.Get, $"products/{id}", cancellationToken: cancellationToken);
        }

        public async Task<ProductDto?> GetProductByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await SendForNullableResultAsync<ProductDto>(HttpMethod.Get, $"products/by-name?name={Uri.EscapeDataString(name)}", cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<ProductDto>> SearchProductsAsync(string name, CancellationToken cancellationToken)
        {
            return await SendForResultAsync<IReadOnlyList<ProductDto>>(HttpMethod.Get, $"products/search?name={Uri.EscapeDataString(name)}", cancellationToken: cancellationToken);
        }

        // Admin products
        public async Task<Guid> CreateProductAsync(CreateProductRequestDto dto, CancellationToken cancellationToken)
        {
            return await SendForResultAsync<Guid>(HttpMethod.Post, "products", dto, cancellationToken);
        }

        public async Task UpdateProductAsync(Guid id, UpdateProductRequestDto dto, CancellationToken cancellationToken)
        {
            await SendNoContentAsync(HttpMethod.Put, $"products/{id}", dto, cancellationToken);
        }

        public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken)
        {
            await SendNoContentAsync(HttpMethod.Delete, $"products/{id}", cancellationToken: cancellationToken);
        }

        public async Task ActivateProductAsync(Guid id, CancellationToken cancellationToken)
        {
            await SendNoContentAsync(HttpMethod.Patch, $"products/{id}/activate", cancellationToken: cancellationToken);
        }

        public async Task DeactivateProductAsync(Guid id, CancellationToken cancellationToken)
        {
            await SendNoContentAsync(HttpMethod.Patch, $"products/{id}/deactivate", cancellationToken: cancellationToken);
        }

        public async Task IncreaseStockAsync(Guid id, int quantity, CancellationToken cancellationToken)
        {
            await SendNoContentAsync(HttpMethod.Patch, $"products/{id}/stock/increase?quantity={quantity}", cancellationToken: cancellationToken);
        }

        public async Task DecreaseStockAsync(Guid id, int quantity, CancellationToken cancellationToken)
        {
            await SendNoContentAsync(HttpMethod.Patch, $"products/{id}/stock/decrease?quantity={quantity}", cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<ProductWithCategoryDto>> GetAllProductsForAdminAsync(int pageNumber, int pageSize, Guid? categoryId, CancellationToken cancellationToken)
        {
            var endpoint = $"products/admin?pageNumber={pageNumber}&pageSize={pageSize}";
            if (categoryId.HasValue)
                endpoint += $"&categoryId={categoryId.Value}";

            return await SendForResultAsync<IReadOnlyList<ProductWithCategoryDto>>(HttpMethod.Get, endpoint, cancellationToken: cancellationToken);
        }

        public async Task<ProductDto?> GetProductByIdForAdminAsync(Guid id, CancellationToken cancellationToken)
        {
            return await SendForNullableResultAsync<ProductDto>(HttpMethod.Get, $"products/admin/{id}", cancellationToken: cancellationToken);
        }

        public async Task<ProductDto?> GetProductByNameForAdminAsync(string name, CancellationToken cancellationToken)
        {
            return await SendForNullableResultAsync<ProductDto>(HttpMethod.Get, $"products/admin/by-name?name={Uri.EscapeDataString(name)}", cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<ProductDto>> SearchProductsForAdminAsync(string name, CancellationToken cancellationToken)
        {
            return await SendForResultAsync<IReadOnlyList<ProductDto>>(HttpMethod.Get, $"products/admin/search?name={Uri.EscapeDataString(name)}", cancellationToken: cancellationToken);
        }
    }
}
