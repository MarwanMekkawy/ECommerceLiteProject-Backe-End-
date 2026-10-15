using Gateway.API.Abstractions;
using Gateway.API.DTOs;

namespace Gateway.API.GraphQL.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class OrderQueries
    {
        // Current user's orders
        public Task<IReadOnlyList<OrderResponseDto>> GetMyOrders(IOrderServiceClient client, int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
            => client.GetCurrentUserOrdersAsync(pageNumber, pageSize, cancellationToken);

        public Task<OrderResponseDto?> GetMyLatestOrder(IOrderServiceClient client, CancellationToken cancellationToken = default)
            => client.GetLatestOrderAsync(cancellationToken);

        public Task<OrderResponseDto?> GetMyOrderById(IOrderServiceClient client, Guid orderId, CancellationToken cancellationToken = default)
            => client.GetCurrentUserOrderByIdAsync(orderId, cancellationToken);


        // Admin
        public Task<IReadOnlyList<OrderResponseDto>> GetAllOrders(IOrderServiceClient client, int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
            => client.GetAllOrdersAsync(pageNumber, pageSize, cancellationToken);

        public Task<OrderResponseDto?> GetOrderByIdForAdmin(IOrderServiceClient client, Guid orderId, CancellationToken cancellationToken = default)
            => client.GetOrderByIdForAdminAsync(orderId, cancellationToken);

        public Task<IReadOnlyList<OrderResponseDto>> GetUserOrdersForAdmin(IOrderServiceClient client, Guid userId, int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
            => client.GetUserOrdersForAdminAsync(userId, pageNumber, pageSize, cancellationToken);
    }
}
