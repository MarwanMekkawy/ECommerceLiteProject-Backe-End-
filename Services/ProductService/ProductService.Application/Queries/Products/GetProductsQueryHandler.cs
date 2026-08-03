using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Queries.Products
{
    public class GetProductsQueryHandler(IProductRepository productRepository) : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
    {
        public async Task<IReadOnlyList<ProductDto>> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken = default)
        {
            var products = await productRepository.GetPaginatedUntrackedAsync(query.PageNumber, query.PageSize, cancellationToken);

            var result = new List<ProductDto>();

            foreach (var product in products)
            {
                result.Add(new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    IsActive = product.IsActive,
                    CategoryId = product.CategoryId
                });
            }

            return result;
        }
    }
}
