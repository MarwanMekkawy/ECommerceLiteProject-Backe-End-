using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Products
{
    public class GetProductByNameQuery : IQuery<ProductDto?>
    {
        public string Name { get; }
        public bool IncludeInactives { get; }

        public GetProductByNameQuery(string name, bool includeInactives = false)
        {
            Name = name;
            IncludeInactives = includeInactives;
        }
    }
}
