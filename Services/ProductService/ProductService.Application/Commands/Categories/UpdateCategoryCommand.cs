using ProductService.Application.Abstractions;


namespace ProductService.Application.Commands.Categories
{
    public class UpdateCategoryCommand : ICommand
    {
        public Guid Id { get; }
        public string NewName { get; }
        public string NewDiscription { get; }

        public UpdateCategoryCommand(Guid id, string newName, string newDiscription)
        {
            Id = id;
            NewName = newName;
            NewDiscription = newDiscription;
        }
    }
}
