using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;
using OrderService.Domain.Enums;
using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;

namespace OrderService.Application.Commands
{
    public class CheckoutOrderCommandHandler(IOrderRepository orderRepository, IProductServiceClient productServiceClient, IUnitOfWork uow)  
        : ICommandHandler<CheckoutOrderCommand, CheckoutOrderDto>
    {
        public async Task<CheckoutOrderDto> HandleAsync(CheckoutOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAndUserIdTrackedAsync(command.OrderId, command.UserId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id [{command.OrderId}] Was NOT FOUND.");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOrderException("Only pending orders can be checked out.");

            var decreasedItems = new List<OrderItem>();

            // container for  current product prices 
            var productPrices = new Dictionary<Guid, (decimal UnitPrice, CurrencyCode Currency)>();

            try
            {
                // Validate products and add current product prices to container
                foreach (var item in order.Items)
                {
                    var product = await productServiceClient.GetProductForCheckoutAsync(item.ProductId, cancellationToken);

                    if (product is null)
                        throw new NotFoundException($"Product with Id [{item.ProductId}] Was NOT FOUND.");

                    productPrices[item.ProductId] = (product.Price, product.Currency);

                    await productServiceClient.DecreaseStockAsync(item.ProductId, item.Quantity, cancellationToken);

                    decreasedItems.Add(item);
                }

                // Confirm order and snapshot prices
                order.Confirm(productPrices, DateTime.UtcNow);

                await uow.SaveChangesAsync(cancellationToken);

                // Return checkout result
                return new CheckoutOrderDto
                {
                    OrderId = order.Id,
                    Items = order.Items.Select(item => new CheckoutOrderItemDto
                    { ProductId = item.ProductId, Quantity = item.Quantity, UnitPrice = item.UnitPrice, Total = item.Total }).ToList(),
                    Total = order.Total,
                    Currency = order.Currency
                };
            }
            catch
            {
                // compenstion if checkout fails
                foreach (var item in decreasedItems)
                {
                    await productServiceClient.IncreaseStockAsync(item.ProductId, item.Quantity, cancellationToken);
                }

                throw;
            }
        }
    }
}
