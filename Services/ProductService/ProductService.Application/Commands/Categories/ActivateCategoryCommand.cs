using ProductService.Application.Abstractions;



namespace ProductService.Application.Commands.Categories
{
    public class ActivateCategoryCommand : ICommand
    {
        public Guid CategoryId { get; }

        public ActivateCategoryCommand(Guid categoryId)
        {
            CategoryId = categoryId;
        }
    }
}
