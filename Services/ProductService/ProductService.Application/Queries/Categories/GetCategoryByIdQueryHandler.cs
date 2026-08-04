using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Queries.Categories
{
    public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository) : IQueryHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        public async Task<CategoryDto?> HandleAsync(GetCategoryByIdQuery query, CancellationToken cancellationToken = default)
        {
            var category = await categoryRepository.GetByIdUntrackedAsync(query.Id, cancellationToken);

            if (category is null)
                throw new NotFoundException("Category not found.");

            if(!query.IncludeInactive && !category.IsActive)
                throw new NotFoundException("Category not found.");

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive,
                Description = category.Description
            };
        }
    }
}
