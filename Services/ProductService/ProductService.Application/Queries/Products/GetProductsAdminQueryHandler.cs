using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Contracts;



namespace ProductService.Application.Queries.Products
{
    public class GetProductsAdminQueryHandler(IProductRepository productRepository) : IQueryHandler<GetProductsAdminQuery, IReadOnlyList<ProductWithCategoryDto>>
    {
        public async Task<IReadOnlyList<ProductWithCategoryDto>> HandleAsync(GetProductsAdminQuery query, CancellationToken cancellationToken = default)
        {
            var products = await productRepository.GetPaginatedUntrackedAsync(query.PageNumber, query.PageSize, query.CategoryId, query.IncludeInactive, cancellationToken);

            var result = new List<ProductWithCategoryDto>();

            foreach (var product in products)
            {
                result.Add(new ProductWithCategoryDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price.Amount,
                    Currency = product.Price.Currency,
                    StockQuantity = product.StockQuantity,
                    IsActive = product.IsActive,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.Name,
                    CategoryIsActive = product.Category.IsActive
                });
            }
            return result;        
        }
    }
}
