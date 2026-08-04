using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Contracts;
using ProductService.Domain.Entities;


namespace ProductService.Application.Queries.Products
{
    public class SearchProductsByNameQueryHandler(IProductRepository productRepository) : IQueryHandler<SearchProductsByNameQuery, IReadOnlyList<ProductDto>>
    {
        public async Task<IReadOnlyList<ProductDto>> HandleAsync(SearchProductsByNameQuery query, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Product> products;

            if (query.IncludeInactive)
                products = await productRepository.SearchByNameIncludeInactiveAsync(query.SearchTerm, cancellationToken);
            else
                products = await productRepository.SearchByNameAsync(query.SearchTerm, cancellationToken);


            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                productDtos.Add(new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price.Amount,
                    Currency = product.Price.Currency,
                    StockQuantity = product.StockQuantity,
                    IsActive = product.IsActive,
                    CategoryId = product.CategoryId
                });
            }
            return productDtos;
        }
    }
}
