using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Queries.Categories
{
    public class SearchCategoriesByNameQueryHandler(ICategoryRepository categoryRepository) : IQueryHandler<SearchCategoriesByNameQuery, IReadOnlyList<CategoryDto>>
    {
        public async Task<IReadOnlyList<CategoryDto>> HandleAsync(SearchCategoriesByNameQuery query, CancellationToken cancellationToken = default)
        {
            var categories = await categoryRepository.SearchByNameAsync(query.SearchTerm, cancellationToken);

            var categoryDtos = new List<CategoryDto>();

            foreach (var category in categories)
            {
                categoryDtos.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                });
            }
            return categoryDtos;
        }
    }
}
