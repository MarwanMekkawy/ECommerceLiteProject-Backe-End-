using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;



namespace ProductService.Application.Queries.Products
{
    public class GetProductsQuery : IQuery<IReadOnlyList<ProductDto>>
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public Guid? CategoryId { get; }
        public bool IncludeInactives { get; }

        public GetProductsQuery(int pageNumber, int pageSize, Guid? categoryId = null, bool includeInactives = false)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            CategoryId = categoryId;
            IncludeInactives = includeInactives;
        }
    }
}
