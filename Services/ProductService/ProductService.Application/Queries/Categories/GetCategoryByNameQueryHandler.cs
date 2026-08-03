using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Queries.Categories
{
    public class GetCategoryByNameQueryHandler(ICategoryRepository categoryRepository) : IQueryHandler<GetCategoryByNameQuery, CategoryDto?>
    {
        public async Task<CategoryDto?> HandleAsync(GetCategoryByNameQuery query, CancellationToken cancellationToken = default)
        {
            var category = await categoryRepository.GetByNameAsync(query.Name, cancellationToken);

            if (category is null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}
