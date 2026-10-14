using Gateway.API.DTOs;

namespace Gateway.API.Abstractions
{
    public interface IOrderServiceClient
    {
        Task<IReadOnlyList<OrderResponseDto>> GetCurrentUserOrdersAsync(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default);
        Task<OrderResponseDto?> GetLatestOrderAsync(CancellationToken cancellationToken = default);
        Task<OrderResponseDto?> GetCurrentUserOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task CreateUserOrderAsync(List<CreateOrderItemDto> items, CancellationToken cancellationToken = default);
        Task AddItemToCartAsync(CreateOrderItemDto request, CancellationToken cancellationToken = default);
        Task IncreaseItemQuantityAsync(Guid orderId, Guid productId, int quantity, CancellationToken cancellationToken = default);
        Task DecreaseItemQuantityAsync(Guid orderId, Guid productId, int quantity, CancellationToken cancellationToken = default);
        Task<CheckoutOrderDto> CheckoutAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task CancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

        // Admin
        Task<IReadOnlyList<OrderResponseDto>> GetAllOrdersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<OrderResponseDto?> GetOrderByIdForAdminAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<OrderResponseDto>> GetUserOrdersForAdminAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
