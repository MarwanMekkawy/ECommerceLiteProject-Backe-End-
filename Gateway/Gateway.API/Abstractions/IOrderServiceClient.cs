using Gateway.API.DTOs;

namespace Gateway.API.Abstractions
{
    public interface IOrderServiceClient
    {
        // Current user's orders and cart
        Task<IReadOnlyList<OrderResponseDto>> GetOrdersAsync(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default);
        Task<OrderResponseDto?> GetLatestOrderAsync(CancellationToken cancellationToken = default);
        Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task CreateOrderAsync(List<CreateOrderItemDto> items, CancellationToken cancellationToken);
        Task AddItemToCartAsync(CreateOrderItemDto request, CancellationToken cancellationToken = default);
        Task IncreaseItemQuantityAsync(Guid orderId, Guid productId, CancellationToken cancellationToken = default);
        Task DecreaseItemQuantityAsync(Guid orderId, Guid productId, CancellationToken cancellationToken = default);
        Task<CheckoutOrderDto> CheckoutAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

        // Admin
        Task<IReadOnlyList<OrderResponseDto>> GetAllOrdersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<OrderResponseDto?> GetOrderByIdForAdminAsync(Guid orderId, CancellationToken cancellationToken);
        Task<IReadOnlyList<OrderResponseDto>> GetUserOrdersForAdminAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
