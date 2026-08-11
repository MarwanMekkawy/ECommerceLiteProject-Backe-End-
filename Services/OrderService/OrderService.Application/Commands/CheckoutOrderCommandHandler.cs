using Domain.Exceptions;
using OrderService.Application.Abstractions;
using OrderService.Application.DTOs;
using OrderService.Domain.Contracts;
using OrderService.Domain.Enums;
using OrderService.Domain.Exceptions.DomainExceptions;
using OrderService.Domain.Orders;

namespace OrderService.Application.Commands
{
    public class CheckoutOrderCommandHandler(IOrderRepository orderRepository, IProductServiceClient productServiceClient) 
        : ICommandHandler<CheckoutOrderCommand, CheckoutOrderDto>
    {
        public async Task<CheckoutOrderDto> HandleAsync(CheckoutOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdUntrackedAsync(command.OrderId, cancellationToken);

            if (order is null)
                throw new NotFoundException($"Order with Id {command.OrderId} Was NOT FOUND.");

            var checkoutItems = new List<CheckoutOrderItemDto>();
            var decreasedItems = new List<OrderItem>();

            try
            {
                CurrencyCode? currency = null;

                foreach (var item in order.Items)
                {
                    var product = await productServiceClient.GetProductForCheckoutAsync(item.ProductId, cancellationToken);

                    if (currency is null)
                        currency = product.Currency;
                    else if (currency != product.Currency)
                        throw new InvalidOrderException("All order items must use the same currency.");

                    await productServiceClient.DecreaseStockAsync(item.ProductId, item.Quantity, cancellationToken);

                    decreasedItems.Add(item);

                    var lineTotal = product.Price * item.Quantity;

                    checkoutItems.Add(new CheckoutOrderItemDto
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        Total = lineTotal
                    });
                }

                var total = checkoutItems.Sum(x => x.Total);

                return new CheckoutOrderDto
                {
                    OrderId = order.Id,
                    Items = checkoutItems,
                    Total = total,
                    Currency = currency!.Value
                };
            }
            catch
            {
                foreach (var item in decreasedItems)
                {
                    await productServiceClient.IncreaseStockAsync(item.ProductId, item.Quantity,  cancellationToken);
                }

                throw;
            }
        }
    }
}
