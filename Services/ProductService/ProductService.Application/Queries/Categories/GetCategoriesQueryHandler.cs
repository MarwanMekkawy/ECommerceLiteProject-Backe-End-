using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Queries.Categories
{
    public class GetCategoriesQueryHandler(ICategoryRepository categoryRepository) : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
    {
        public async Task<IReadOnlyList<CategoryDto>> HandleAsync(GetCategoriesQuery query, CancellationToken cancellationToken = default)
        {
            var categories = await categoryRepository.GetPaginatedUntrackedAsync(query.PageNumber, query.PageSize, cancellationToken);

            var result = new List<CategoryDto>();

            foreach (var category in categories) 
            {
                result.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                });
            }
            return result;
        }
    }
}
