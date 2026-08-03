using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Abstractions;
using ProductService.Application.Commands.Products;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.Products;
using System.Threading;

namespace ProductService.API.Controllers
{
    /// <summary>
    /// Provides endpoints for retrieving and managing products, including
    /// product creation, updates, deletion, activation, deactivation, and stock management.
    /// Product retrieval endpoints are publicly accessible, while management operations
    /// require administrator authorization.
    /// </summary>
    [Route("api/V1/products")]
    [ApiController]
    public class ProductsController(
        IQueryHandler<GetProductsQuery, IReadOnlyList<ProductDto>> getProducts,
        IQueryHandler<GetProductByIdQuery, ProductDto?> getProductById,
        IQueryHandler<GetProductByNameQuery, ProductDto?> getProductByName,
        IQueryHandler<SearchProductsByNameQuery, IReadOnlyList<ProductDto>> searchProducts,
        ICommandHandler<CreateProductCommand, Guid> createProduct,
        ICommandHandler<UpdateProductCommand> updateProduct,
        ICommandHandler<DeleteProductCommand> deleteProduct,
        ICommandHandler<ActivateProductCommand> activateProduct,
        ICommandHandler<DeactivateProductCommand> deactivateProduct,
        ICommandHandler<IncreaseStockCommand, int> increaseStock,
        ICommandHandler<DecreaseStockCommand, int> decreaseStock)
        : ControllerBase

    {
        /// <summary>
        /// Retrieves a paginated list of products.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Defaults to 1.</param>
        /// <param name="pageSize">The number of products per page. Defaults to 10.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A paginated list of products.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetProductsQuery(pageNumber, pageSize);

            var result = await getProducts.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The requested product.</returns>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery(id);

            var result = await getProductById.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a product by its name.
        /// </summary>
        /// <param name="name">The name of the product to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The requested product.</returns>
        [HttpGet("by-name/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductByName(string name, CancellationToken cancellationToken)
        {
            var query = new GetProductByNameQuery(name);

            var result = await getProductByName.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Searches for products whose names match the specified search term.
        /// </summary>
        /// <param name="name">The search term to use when searching product names.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A list of products matching the search term.</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchProducts([FromQuery] string name, CancellationToken cancellationToken)
        {
            var query = new SearchProductsByNameQuery(name);

            var result = await searchProducts.HandleAsync(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="dto">The information required to create the product.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The unique identifier of the newly created product.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequestDto dto, CancellationToken cancellationToken = default)
        {
            var command = new CreateProductCommand(dto.Name, dto.Description, dto.Amount, dto.Discount, dto.Currency, dto.CategoryId, dto.StockQuantity);

            var result = await createProduct.HandleAsync(command, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="dto">The updated product information.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the product was updated successfully.</returns>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequestDto dto, CancellationToken cancellationToken)
        {
            var command = new UpdateProductCommand(id, dto.Name, dto.Description, dto.Amount, dto.Currency, dto.Discount, dto.CategoryId);

            await updateProduct.HandleAsync(command, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Deletes an existing product.
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the product was deleted successfully.</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteProductCommand(id);

            await deleteProduct.HandleAsync(command, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Activates an existing product, making it available as an active product.
        /// </summary>
        /// <param name="id">The unique identifier of the product to activate.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the product was activated successfully.</returns>
        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateProduct(Guid id, CancellationToken cancellationToken)
        {
            var command = new ActivateProductCommand(id);

            await activateProduct.HandleAsync(command, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Deactivates an existing product, making it inactive.
        /// </summary>
        /// <param name="id">The unique identifier of the product to deactivate.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the product was deactivated successfully.</returns>
        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateProduct(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeactivateProductCommand(id);

            await deactivateProduct.HandleAsync(command,cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Increases the stock quantity of an existing product.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <param name="quantity">The quantity to add to the current stock.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the stock was increased successfully.</returns>
        [HttpPatch("{id:guid}/stock/increase")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IncreaseStock(Guid id, [FromQuery] int quantity, CancellationToken cancellationToken)
        {
            var command = new IncreaseStockCommand(id, quantity);

            await increaseStock.HandleAsync(command, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Decreases the stock quantity of an existing product.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <param name="quantity">The quantity to subtract from the current stock.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the stock was decreased successfully.</returns>
        [HttpPatch("{id:guid}/stock/decrease")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DecreaseStock(Guid id, [FromQuery] int quantity, CancellationToken cancellationToken)
        {
            var command = new DecreaseStockCommand(id, quantity);

            await decreaseStock.HandleAsync(command, cancellationToken);

            return Ok();
        }
    }
}
