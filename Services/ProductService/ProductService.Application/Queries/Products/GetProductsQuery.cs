using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;


namespace ProductService.Application.Queries.Products
{
    public class GetProductsQuery : IQuery<IReadOnlyList<ProductDto>>
    {
        public int PageNumber { get;}
        public int PageSize {  get;}
        public GetProductsQuery(int pageNumber, int pageSize) 
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
