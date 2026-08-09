namespace OrderService.Application.Abstractions
{
    public interface IProductServiceClient
    {
        Task DecreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken);  // cancellation token??
        Task IncreaseStockAsync(Guid productId, int quantity, CancellationToken cancellationToken);
    }
}
