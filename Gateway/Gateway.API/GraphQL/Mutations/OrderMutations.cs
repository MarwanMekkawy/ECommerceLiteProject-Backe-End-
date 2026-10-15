using Gateway.API.Abstractions;
using Gateway.API.DTOs;

namespace Gateway.API.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class OrderMutations
    {
        // Cart
        public Task CreateUserOrder(IOrderServiceClient client, List<CreateOrderItemDto> items, CancellationToken cancellationToken = default)
            => client.CreateUserOrderAsync(items, cancellationToken);

        public Task AddItemToCart(IOrderServiceClient client, CreateOrderItemDto item, CancellationToken cancellationToken = default)
            => client.AddItemToCartAsync(item, cancellationToken);

        public Task IncreaseItemQuantity(IOrderServiceClient client, Guid orderId, Guid productId, int quantity, CancellationToken cancellationToken = default)
            => client.IncreaseItemQuantityAsync(orderId, productId, quantity, cancellationToken);

        public Task DecreaseItemQuantity(IOrderServiceClient client, Guid orderId, Guid productId, int quantity, CancellationToken cancellationToken = default)
            => client.DecreaseItemQuantityAsync(orderId, productId, quantity, cancellationToken);


        // Checkout
        public Task<CheckoutOrderDto> CheckoutOrder(IOrderServiceClient client, Guid orderId, CancellationToken cancellationToken = default)
            => client.CheckoutAsync(orderId, cancellationToken);

        public Task CancelOrder(IOrderServiceClient client, Guid orderId, CancellationToken cancellationToken = default)
            => client.CancelOrderAsync(orderId, cancellationToken);
    }
}
