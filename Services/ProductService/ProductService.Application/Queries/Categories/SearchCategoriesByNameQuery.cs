using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Categories
{
    public class SearchCategoriesByNameQuery : IQuery<IReadOnlyList<CategoryDto>>
    {
        public string SearchTerm { get; }

        public SearchCategoriesByNameQuery(string searchTerm)
        {
            SearchTerm = searchTerm;
        }
    }
}
