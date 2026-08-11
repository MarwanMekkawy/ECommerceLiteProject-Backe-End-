using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OrderService.InfraStructure.Clients
{
    public class ProductServiceClient(HttpClient httpClient, IServiceTokenClient serviceTokenClient) : IProductServiceClient
    {
        public async Task DecreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new HttpRequestMessage(HttpMethod.Patch, $"{productId}/stock/decrease?quantity={quantity}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();        
        }

        public async Task IncreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new HttpRequestMessage(HttpMethod.Patch, $"{productId}/stock/increase?quantity={quantity}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task<ProductDto> GetProductForCheckoutAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{productId}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);

            if (result is null)
                throw new NotFoundException("ProductService Didnt find this product.");

            return result;
        }
    }
}
