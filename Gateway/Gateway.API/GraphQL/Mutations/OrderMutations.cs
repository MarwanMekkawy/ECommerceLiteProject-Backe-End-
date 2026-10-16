using Gateway.API.Abstractions;
using Gateway.API.DTOs;

namespace Gateway.API.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class OrderMutations
    {
        // Cart
        public async Task<OrderMutationPayload> CreateUserOrder(IOrderServiceClient client, List<CreateOrderItemDto> items, CancellationToken cancellationToken = default)
        { 
            await client.CreateUserOrderAsync(items, cancellationToken); 
            return new OrderMutationPayload(true); 
        }

        public async Task<OrderMutationPayload> AddItemToCart(IOrderServiceClient client, CreateOrderItemDto item, CancellationToken cancellationToken = default) 
        { 
            await client.AddItemToCartAsync(item, cancellationToken); 
            return new OrderMutationPayload(true); 
        }
        public async Task<OrderMutationPayload> IncreaseItemQuantity(IOrderServiceClient client, Guid orderId, Guid productId, int quantity, CancellationToken cancellationToken = default) 
        {
            await client.IncreaseItemQuantityAsync(orderId, productId, quantity, cancellationToken); 
            return new OrderMutationPayload(true); 
        }
        public async Task<OrderMutationPayload> DecreaseItemQuantity(IOrderServiceClient client, Guid orderId, Guid productId, int quantity, CancellationToken cancellationToken = default) 
        { 
            await client.DecreaseItemQuantityAsync(orderId, productId, quantity, cancellationToken); 
            return new OrderMutationPayload(true); 
        }


        // Checkout
        public Task<CheckoutOrderDto> CheckoutOrder(IOrderServiceClient client, Guid orderId, CancellationToken cancellationToken = default)
            => client.CheckoutAsync(orderId, cancellationToken);

        public async Task<OrderMutationPayload> CancelOrder(IOrderServiceClient client, Guid orderId, CancellationToken cancellationToken = default) 
        { 
            await client.CancelOrderAsync(orderId, cancellationToken); 
            return new OrderMutationPayload(true); 
        }
    }
    public sealed record OrderMutationPayload(bool Success);
}
