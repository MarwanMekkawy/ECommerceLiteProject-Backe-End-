using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Products
{
    public class GetProductByNameQuery : IQuery<ProductDto?>
    {
        public string Name { get; }

        public GetProductByNameQuery(string name)
        {
            Name = name;
        }
    }
}
