using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Categories
{
    public class GetCategoriesQuery : IQuery<IReadOnlyList<CategoryDto>>
    {
        public int PageNumber { get; }
        public int PageSize { get; }

        public GetCategoriesQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
