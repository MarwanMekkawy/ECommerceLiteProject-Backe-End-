using Gateway.API.Abstractions;
using Gateway.API.DTOs;
using HotChocolate.Authorization;

namespace Gateway.API.GraphQL.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class OrderQueries
    {
        // Current user's orders
        [Authorize]
        public Task<IReadOnlyList<OrderResponseDto>> GetCurrentUserOrders(IOrderServiceClient client, int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
            => client.GetCurrentUserOrdersAsync(pageNumber, pageSize, cancellationToken);
        [Authorize]
        public Task<OrderResponseDto?> GetCurrentUserLatestOrder(IOrderServiceClient client, CancellationToken cancellationToken = default)
            => client.GetLatestOrderAsync(cancellationToken);
        [Authorize]
        public Task<OrderResponseDto?> GetCurrentUserOrderById(IOrderServiceClient client, Guid orderId, CancellationToken cancellationToken = default)
            => client.GetCurrentUserOrderByIdAsync(orderId, cancellationToken);


        // Admin
        [Authorize(Roles = ["Admin"])]
        public Task<IReadOnlyList<OrderResponseDto>> GetAllOrders(IOrderServiceClient client, int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
            => client.GetAllOrdersAsync(pageNumber, pageSize, cancellationToken);
        [Authorize(Roles = ["Admin"])]
        public Task<OrderResponseDto?> GetOrderByIdForAdmin(IOrderServiceClient client, Guid orderId, CancellationToken cancellationToken = default)
            => client.GetOrderByIdForAdminAsync(orderId, cancellationToken);
        [Authorize(Roles = ["Admin"])]
        public Task<IReadOnlyList<OrderResponseDto>> GetUserOrdersForAdmin(IOrderServiceClient client, Guid userId, int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
            => client.GetUserOrdersForAdminAsync(userId, pageNumber, pageSize, cancellationToken);
    }
}
