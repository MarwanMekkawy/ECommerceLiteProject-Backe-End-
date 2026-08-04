using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;



namespace ProductService.Application.Queries.Categories
{
    public class GetCategoryByIdQuery : IQuery<CategoryDto?>
    {
        public Guid Id { get; }
        public bool IncludeInactive { get; }

        public GetCategoryByIdQuery(Guid id, bool includeInactive = false)
        {
            Id = id;
            IncludeInactive = includeInactive;
        }
    }
}
