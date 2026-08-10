using OrderService.Application.Abstractions;

namespace OrderService.InfraStructure.Clients
{
    public class ProductServiceClient(HttpClient httpClient) : IProductServiceClient
    {
        public async Task DecreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken)
        {
            var response = await httpClient.PatchAsync($"{productId}/stock/decrease?quantity={quantity}", content: null,cancellationToken);

            response.EnsureSuccessStatusCode();
        }

        public async Task IncreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken)
        {
            var response = await httpClient.PatchAsync($"{productId}/stock/increase?quantity={quantity}", content: null, cancellationToken);

            response.EnsureSuccessStatusCode();
        }
    }
}
