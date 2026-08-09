namespace OrderService.Application.Abstractions
{
    public interface IProductServiceClient
    {
        Task DecreaseStockAsync(Guid productId, int quantity);
        Task IncreaseStockAsync(Guid productId, int quantity);
    }
}
