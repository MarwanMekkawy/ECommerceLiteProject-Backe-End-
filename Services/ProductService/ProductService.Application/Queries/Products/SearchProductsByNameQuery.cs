using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Products
{
    public class SearchProductsByNameQuery : IQuery<IReadOnlyList<ProductDto>>
    {
        public string SearchTerm { get; }
        public bool IncludeInactive { get; }

        public SearchProductsByNameQuery(string searchTerm, bool includeInactive = false)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term is required.");
            SearchTerm = searchTerm;
            IncludeInactive = includeInactive;
        }
    }
}
