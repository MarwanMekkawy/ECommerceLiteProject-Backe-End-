using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Contracts;
using ProductService.Domain.Entities;


namespace ProductService.Application.Queries.Categories
{
    public class SearchCategoriesByNameQueryHandler(ICategoryRepository categoryRepository) : IQueryHandler<SearchCategoriesByNameQuery, IReadOnlyList<CategoryDto>>
    {
        public async Task<IReadOnlyList<CategoryDto>> HandleAsync(SearchCategoriesByNameQuery query, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Category> categories;
            if (query.IncludeInactive)
                 categories = await categoryRepository.SearchByNameIncludingInactiveAsync(query.SearchTerm, cancellationToken);
            else
                categories = await categoryRepository.SearchByNameAsync(query.SearchTerm, cancellationToken);

            var categoryDtos = new List<CategoryDto>();

            foreach (var category in categories)
            {
                categoryDtos.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsActive = category.IsActive,
                    Description = category.Description
                });
            }
            return categoryDtos;
        }
    }
}
