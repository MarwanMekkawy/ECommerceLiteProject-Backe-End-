using Microsoft.AspNetCore.Mvc;
using OrderService.API.ApiClaimsFactory;
using OrderService.Application.Abstractions;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using OrderService.Domain.Orders;

namespace OrderService.API.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    public class OrdersController(
        ICommandHandler<CreateOrderCommand> createOrderHandler,
        ICommandHandler<CheckoutOrderCommand, CheckoutOrderDto> checkoutOrderHandler,
        ICommandHandler<CompleteOrderInternalCommand> completeOrderInternalHandler,
        ICommandHandler<CancelOrderCommand> cancelOrderHandler,
        ICommandHandler<CancelOrderInternalCommand> cancelOrderInternalHandler,
        IQueryHandler<GetOrderByIdQuery, Order?> getOrderByIdHandler,
        IQueryHandler<GetOrdersByUserQuery, IReadOnlyList<Order>> getOrdersByUserHandler,
        IQueryHandler<GetLatestOrderQuery, Order?> getLatestOrderHandler,
        IQueryHandler<GetAllOrdersQuery, IReadOnlyList<Order>> getAllOrdersHandler,
        IQueryHandler<GetOrderByIdAdminQuery, Order?> getOrderByIdAdminHandler) 
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserOrders(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var query = new GetOrdersByUserQuery(claims.UserId, pageNumber, pageSize);

            var orders = await getOrdersByUserHandler.HandleAsync(query, cancellationToken);

            return Ok(orders);
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetCurrentUserLatestOrder(CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var query = new GetLatestOrderQuery(claims.UserId);

            var order = await getLatestOrderHandler.HandleAsync(query, cancellationToken);

            return order is null ? NotFound() : Ok(order);
        }


        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetCurrentUserOrderById(Guid orderId, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var query = new GetOrderByIdQuery(claims.UserId, orderId);

            var order = await getOrderByIdHandler.HandleAsync(query, cancellationToken);

            return order is null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserOrder([FromBody] List<CreateOrderItemDto> items, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var command = new CreateOrderCommand(claims.UserId, items);

            await createOrderHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }

        [HttpPost("{orderId:guid}/checkout")]
        public async Task<IActionResult> CheckOutOrder(Guid orderId, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var command = new CheckoutOrderCommand(claims.UserId, orderId);

            var result = await checkoutOrderHandler.HandleAsync(command, cancellationToken);

            return Ok(result);
        }

        [HttpPost("{orderId:guid}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var command = new CancelOrderCommand(claims.UserId, orderId);

            await cancelOrderHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }

        //Admin
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllOrders(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
        {
            var query = new GetAllOrdersQuery(pageNumber, pageSize);

            var orders = await getAllOrdersHandler.HandleAsync(query, cancellationToken);

            return Ok(orders);
        }

        [HttpGet("admin/{orderId:guid}")]
        public async Task<IActionResult> GetOrderById(Guid orderId, CancellationToken cancellationToken = default)
        {
            var query = new GetOrderByIdAdminQuery(orderId);

            var order = await getOrderByIdAdminHandler.HandleAsync(query, cancellationToken);

            return order is null ? NotFound() : Ok(order);
        }

        [HttpGet("admin/user/{userId:guid}")]
        public async Task<IActionResult> GetOrdersByUser(Guid userId, int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
        {
            var query = new GetOrdersByUserQuery(userId, pageNumber, pageSize);

            var orders = await getOrdersByUserHandler.HandleAsync(query, cancellationToken);

            return Ok(orders);
        }

        //service-service endpoints
        [HttpPost("{orderId:guid}/complete-internal")]
        public async Task<IActionResult> CompleteOrderInternal(Guid orderId, CancellationToken cancellationToken = default)
        {
            var command = new CompleteOrderInternalCommand(orderId);

            await completeOrderInternalHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }

        [HttpPost("{orderId:guid}/cancel-internal")]
        public async Task<IActionResult> CancelOrderInternal(Guid orderId, CancellationToken cancellationToken = default)
        {
            var command = new CancelOrderInternalCommand(orderId);

            await cancelOrderInternalHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }
    }
}
