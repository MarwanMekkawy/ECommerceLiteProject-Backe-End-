using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Products
{
    public class GetProductByIdQuery : IQuery<ProductDto?>
    {
        public Guid ProductId { get; }
        public bool IncludeInactive { get; }

        public GetProductByIdQuery(Guid productId, bool includeInactive = false)
        {
            ProductId = productId;
            IncludeInactive = includeInactive;
        }
    }
}
