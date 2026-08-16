using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OrderService.InfraStructure.Clients
{
    public class ProductServiceClient(HttpClient httpClient, IServiceTokenClient serviceTokenClient) : IProductServiceClient
    {
        public async Task DecreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new HttpRequestMessage(HttpMethod.Patch, $"products/{productId}/stock/decrease?quantity={quantity}");

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
                throw new Exception(error ?? $"ProductService returned {(int)response.StatusCode} {response.StatusCode}");
            }
        }

        public async Task IncreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new HttpRequestMessage(HttpMethod.Patch, $"products/{productId}/stock/increase?quantity={quantity}");

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
                throw new Exception(error ?? $"ProductService returned {(int)response.StatusCode} {response.StatusCode}");
            }
        }

        public async Task<ProductDto> GetProductForCheckoutAsync(Guid productId, CancellationToken cancellationToken = default)
        {

            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            using var request = new HttpRequestMessage(HttpMethod.Get, $"products/{productId}");

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
                throw new Exception(error ?? $"ProductService returned {(int)response.StatusCode} {response.StatusCode}");
            }

            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());

            var result = await response.Content.ReadFromJsonAsync<ProductDto>(options, cancellationToken);

            if (result is null)
                throw new NotFoundException("ProductService Didnt find this product.");

            return result;
        }
    }
}
