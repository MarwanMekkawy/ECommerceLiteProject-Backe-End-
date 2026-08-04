using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Categories
{
    public class SearchCategoriesByNameQuery : IQuery<IReadOnlyList<CategoryDto>>
    {
        public string SearchTerm { get; }
        public bool IncludeInactive { get; }


        public SearchCategoriesByNameQuery(string searchTerm, bool includeInactive = false)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term is required.");

            SearchTerm = searchTerm;
            IncludeInactive = includeInactive;
        }
    }
}
