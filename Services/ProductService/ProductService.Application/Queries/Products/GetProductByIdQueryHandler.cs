using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Queries.Products
{
    public class GetProductByIdQueryHandler(IProductRepository productRepository) : IQueryHandler<GetProductByIdQuery, ProductDto?>
    {
        public async Task<ProductDto?> HandleAsync(GetProductByIdQuery query,CancellationToken cancellationToken = default)
        {
            var product = await productRepository.GetByIdUntrackedAsync(query.ProductId, cancellationToken);

            if (product is null)
                throw new NotFoundException("Product not found.");

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId
            };
        }
    }
}
