using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Products
{
    public class GetProductByIdQuery : IQuery<ProductDto?>
    {
        public Guid ProductId { get; }
        public bool IncludeInactives { get; }

        public GetProductByIdQuery(Guid productId, bool includeInactives = false)
        {
            ProductId = productId;
            IncludeInactives = includeInactives;
        }
    }
}
