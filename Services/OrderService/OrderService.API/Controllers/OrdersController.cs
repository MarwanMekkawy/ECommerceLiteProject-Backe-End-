using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.ApiClaimsFactory;
using OrderService.Application.Abstractions;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;

namespace OrderService.API.Controllers
{
    /// <summary>
    /// Provides endpoints for retrieving and managing orders for authenticated users,
    /// administrators, and internal service-to-service operations.
    /// </summary>
    [Route("api/v1/orders")]
    [ApiController]
    public class OrdersController(
        ICommandHandler<CreateOrderCommand> createOrderHandler,
        ICommandHandler<CheckoutOrderCommand, CheckoutOrderDto> checkoutOrderHandler,
        ICommandHandler<CompleteOrderInternalCommand> completeOrderInternalHandler,
        ICommandHandler<CancelOrderCommand> cancelOrderHandler,
        ICommandHandler<CancelOrderInternalCommand> cancelOrderInternalHandler,
        ICommandHandler<AddOrderItemCommand> addOrderItemHandler,
        ICommandHandler<DecreaseOrderItemCommand> decreaseOrderItemHandler,
        ICommandHandler<IncreaseOrderItemCommand> increaseOrderItemHandler,
        IQueryHandler<GetOrderByIdQuery, OrderResponseDto?> getOrderByIdHandler,
        IQueryHandler<GetOrdersByUserQuery, IReadOnlyList<OrderResponseDto>> getOrdersByUserHandler,
        IQueryHandler<GetLatestOrderQuery, OrderResponseDto?> getLatestOrderHandler,
        IQueryHandler<GetAllOrdersQuery, IReadOnlyList<OrderResponseDto>> getAllOrdersHandler,
        IQueryHandler<GetOrderByIdAdminQuery, OrderResponseDto?> getOrderByIdAdminHandler)
        : ControllerBase
    {
        /// <summary>
        /// Retrieves a paginated list of orders belonging to the currently authenticated user.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Defaults to 1.</param>
        /// <param name="pageSize">The number of orders per page. Defaults to 5.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A paginated list of the current user's orders.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserOrders(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var query = new GetOrdersByUserQuery(claims.UserId, pageNumber, pageSize);

            var orders = await getOrdersByUserHandler.HandleAsync(query, cancellationToken);

            return Ok(orders);
        }

        /// <summary>
        /// Retrieves the latest order belonging to the currently authenticated user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The latest order, or <see cref="NotFoundResult"/> if no order exists.</returns>
        [HttpGet("latest")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserLatestOrder(CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var query = new GetLatestOrderQuery(claims.UserId);

            var order = await getLatestOrderHandler.HandleAsync(query, cancellationToken);

            return order is null ? NotFound() : Ok(order);
        }

        /// <summary>
        /// Retrieves an order by its unique identifier for the currently authenticated user.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The requested order, or <see cref="NotFoundResult"/> if the order does not exist.</returns>
        [HttpGet("{orderId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserOrderById(Guid orderId, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var query = new GetOrderByIdQuery(claims.UserId, orderId);

            var order = await getOrderByIdHandler.HandleAsync(query, cancellationToken);

            return order is null ? NotFound() : Ok(order);
        }

        /// <summary>
        /// Adds multiple products to the currently authenticated user's cart.
        /// If the user does not have a pending order, one is created automatically.
        /// If a pending order already exists, the specified items are added to it.
        /// Existing products have their quantities increased.
        /// </summary>
        /// <param name="items">The products and quantities to add to the cart.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the items were added successfully.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUserOrder([FromBody] List<CreateOrderItemDto> items, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var command = new CreateOrderCommand(claims.UserId, items);

            await createOrderHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Adds a single product to the currently authenticated user's cart.
        /// If the user does not have a pending order, one is created automatically.
        /// If the product already exists in the cart, its quantity is increased.
        /// </summary>
        /// <param name="item">The product and quantity to add to the cart.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the item was added successfully.</returns>
        [HttpPost("item")]
        [Authorize]
        public async Task<IActionResult> AddOrderItem([FromBody] CreateOrderItemDto item, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var command = new AddOrderItemCommand(claims.UserId, item.ProductId, item.Quantity);

            await addOrderItemHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Increases the quantity of an existing product in the user's order.
        /// The product must already exist in the order; otherwise, an exception is thrown.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="productId">The ID of the product to increase.</param>
        /// <param name="quantity">The quantity to add.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the quantity was increased successfully.</returns>
        [HttpPost("{orderId:guid}/items/{productId:guid}/increase")]
        [Authorize]
        public async Task<IActionResult> IncreaseOrderItem(Guid orderId, Guid productId, [FromBody] int quantity, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var command = new IncreaseOrderItemCommand(claims.UserId, orderId, productId, quantity);

            await increaseOrderItemHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Decreases the quantity of an existing product in the user's order.
        /// The product must already exist in the order.
        /// If the quantity reaches zero, the order item is removed.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="productId">The ID of the product to decrease.</param>
        /// <param name="quantity">The quantity to remove.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the quantity was decreased successfully.</returns>
        [HttpPost("{orderId:guid}/items/{productId:guid}/decrease")]
        [Authorize]
        public async Task<IActionResult> DecreaseOrderItem(Guid orderId, Guid productId, [FromBody] int quantity, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var command = new DecreaseOrderItemCommand(claims.UserId, orderId, productId, quantity);

            await decreaseOrderItemHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Checks out an order belonging to the currently authenticated user.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to check out.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The checkout result.</returns>
        [HttpPost("{orderId:guid}/checkout")]
        [Authorize]
        public async Task<IActionResult> CheckOutOrder(Guid orderId, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var command = new CheckoutOrderCommand(claims.UserId, orderId);

            var result = await checkoutOrderHandler.HandleAsync(command, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Cancels an order belonging to the currently authenticated user.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to cancel.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the order was cancelled successfully.</returns>
        [HttpPost("{orderId:guid}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(Guid orderId, CancellationToken cancellationToken = default)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var command = new CancelOrderCommand(claims.UserId, orderId);

            await cancelOrderHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }

        //Admin
        /// <summary>
        /// Retrieves a paginated list of all orders.
        /// This endpoint is intended for administrator use.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Defaults to 1.</param>
        /// <param name="pageSize">The number of orders per page. Defaults to 5.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A paginated list of all orders.</returns>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders(int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
        {
            var query = new GetAllOrdersQuery(pageNumber, pageSize);

            var orders = await getAllOrdersHandler.HandleAsync(query, cancellationToken);

            return Ok(orders);
        }

        /// <summary>
        /// Retrieves an order by its unique identifier.
        /// This endpoint is intended for administrator use.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The requested order, or <see cref="NotFoundResult"/> if the order does not exist.</returns>
        [HttpGet("admin/{orderId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderById(Guid orderId, CancellationToken cancellationToken = default)
        {
            var query = new GetOrderByIdAdminQuery(orderId);

            var order = await getOrderByIdAdminHandler.HandleAsync(query, cancellationToken);

            return order is null ? NotFound() : Ok(order);
        }

        /// <summary>
        /// Retrieves a paginated list of orders belonging to a specified user.
        /// This endpoint is intended for administrator use.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose orders should be retrieved.</param>
        /// <param name="pageNumber">The page number to retrieve. Defaults to 1.</param>
        /// <param name="pageSize">The number of orders per page. Defaults to 5.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A paginated list of the user's orders.</returns>
        [HttpGet("admin/user/{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersByUser(Guid userId, int pageNumber = 1, int pageSize = 5, CancellationToken cancellationToken = default)
        {
            var query = new GetOrdersByUserQuery(userId, pageNumber, pageSize);

            var orders = await getOrdersByUserHandler.HandleAsync(query, cancellationToken);

            return Ok(orders);
        }

        //service-service endpoints  //@ add [Authorize] with service authintication
        /// <summary>
        /// Completes an order through an internal service-to-service request.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to complete.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the order was completed successfully.</returns>
        [HttpPost("{orderId:guid}/complete-internal")]
        public async Task<IActionResult> CompleteOrderInternal(Guid orderId, CancellationToken cancellationToken = default)
        {
            var command = new CompleteOrderInternalCommand(orderId);

            await completeOrderInternalHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }

        //@ ========= probably delete the endpoint if i dont use it cuz the only internal cancellation will be automatic after order expiration =========
        /// <summary>
        /// Cancels an order through an internal service-to-service request.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to cancel.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the order was cancelled successfully.</returns>
        [HttpPost("{orderId:guid}/cancel-internal")]
        public async Task<IActionResult> CancelOrderInternal(Guid orderId, CancellationToken cancellationToken = default)
        {
            var command = new CancelOrderInternalCommand(orderId);

            await cancelOrderInternalHandler.HandleAsync(command, cancellationToken);

            return NoContent();
        }
        // ==============================================================================================================================================
    }
}
