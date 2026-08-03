using ProductService.Application.Abstractions;
using ProductService.Application.DTOs;


namespace ProductService.Application.Queries.Categories
{
    public class GetCategoryByNameQuery : IQuery<CategoryDto?>
    {
        public string Name { get; }

        public GetCategoryByNameQuery(string name)
        {
            Name = name;
        }
    }
}
