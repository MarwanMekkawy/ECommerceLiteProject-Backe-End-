using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.Products;
using ProductService.Domain.Contracts;


public class GetProductByNameQueryHandler(IProductRepository productRepository) : IQueryHandler<GetProductByNameQuery, ProductDto?>
{
    public async Task<ProductDto?> HandleAsync(GetProductByNameQuery query,CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByNameAsync(query.Name, cancellationToken);

        if (product is null)
            return null;

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
