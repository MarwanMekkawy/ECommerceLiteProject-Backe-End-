using ProductService.Application.Abstractions;


namespace ProductService.Application.Commands.Categories
{
    public class DeactivateCategoryCommand : ICommand
    {
        public Guid CategoryId { get; }

        public DeactivateCategoryCommand(Guid categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
