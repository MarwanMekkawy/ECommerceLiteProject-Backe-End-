using OrderService.Application.Abstractions;
using System.Net.Http.Headers;

namespace OrderService.InfraStructure.Clients
{
    public class ProductServiceClient(HttpClient httpClient, IServiceTokenClient serviceTokenClient) : IProductServiceClient
    {
        public async Task DecreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            {
                var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

                var request = new HttpRequestMessage(HttpMethod.Patch, $"{productId}/stock/decrease?quantity={quantity}");

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.SendAsync(request, cancellationToken);

                response.EnsureSuccessStatusCode();
            }
        }

        public async Task IncreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            var token = await serviceTokenClient.GetTokenAsync(cancellationToken);

            var request = new HttpRequestMessage(HttpMethod.Patch, $"{productId}/stock/increase?quantity={quantity}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
        }
    }
}
