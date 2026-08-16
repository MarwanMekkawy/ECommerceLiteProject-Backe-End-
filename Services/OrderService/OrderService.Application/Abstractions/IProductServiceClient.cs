using OrderService.Application.DTOs;

namespace OrderService.Application.Abstractions
{
    public interface IProductServiceClient
    {
        Task DecreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
        Task IncreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
        Task<ProductDto> GetProductForCheckoutAsync(Guid productId,CancellationToken cancellationToken = default);
    }
}
