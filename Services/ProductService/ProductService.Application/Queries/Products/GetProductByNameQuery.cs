using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Products
{
    public class GetProductByNameQuery : IQuery<ProductDto?>
    {
        public string Name { get; }
        public bool IncludeInactive { get; }

        public GetProductByNameQuery(string name, bool includeInactive = false)
        {
            Name = name;
            IncludeInactive = includeInactive;
        }
    }
}
