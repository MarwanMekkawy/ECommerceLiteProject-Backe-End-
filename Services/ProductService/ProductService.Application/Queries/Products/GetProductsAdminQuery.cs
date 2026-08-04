using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;



namespace ProductService.Application.Queries.Products
{
    public class GetProductsAdminQuery : IQuery<IReadOnlyList<ProductWithCategoryDto>>
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public Guid? CategoryId { get; }
        public bool IncludeInactive { get; }

        public GetProductsAdminQuery(int pageNumber, int pageSize, Guid? categoryId = null, bool includeInactive = false)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            CategoryId = categoryId;
            IncludeInactive = includeInactive;
        }
    }
}
