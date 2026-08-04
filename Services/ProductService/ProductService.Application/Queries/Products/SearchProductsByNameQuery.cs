using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Products
{
    public class SearchProductsByNameQuery : IQuery<IReadOnlyList<ProductDto>>
    {
        public string SearchTerm { get; }
        public bool IncludeInactives { get; }

        public SearchProductsByNameQuery(string searchTerm, bool includeInactives = false)
        {
            SearchTerm = searchTerm;
            IncludeInactives = includeInactives;
        }
    }
}
