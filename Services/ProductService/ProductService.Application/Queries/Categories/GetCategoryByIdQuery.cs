using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;



namespace ProductService.Application.Queries.Categories
{
    public class GetCategoryByIdQuery : IQuery<CategoryDto?>
    {
        public Guid Id;

        public GetCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
