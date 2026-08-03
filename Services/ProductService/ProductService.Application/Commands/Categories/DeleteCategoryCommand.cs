using ProductService.Application.Abstractions;


namespace ProductService.Application.Commands.Categories
{
    public class DeleteCategoryCommand : ICommand
    {
        public Guid Id { get;}
        public DeleteCategoryCommand(Guid id)
        {
            Id = id;
        }
    }
}
