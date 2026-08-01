using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;
using ProductService.Domain.Entities;


namespace ProductService.Application.Commands.Categories
{
    public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork  uow) : ICommandHandler<CreateCategoryCommand, Guid>
    {
        public async Task<Guid> HandleAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
        {
            Category category = new Category(command.Name, command.Description);

            await categoryRepository.AddAsync(category, cancellationToken);

            await uow.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}
