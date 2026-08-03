using ProductService.Application.Abstractions;



namespace ProductService.Application.Commands.Categories
{
    public class CreateCategoryCommand : ICommand<Guid>
    {
        public string Name { get; }
        public string? Description { get; }

        public CreateCategoryCommand(string name, string? description)
        {
            Name = name;
            Description = description;
        }
    }
}
