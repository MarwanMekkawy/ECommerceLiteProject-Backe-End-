using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Commands.Categories
{
    public class UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork uow) : ICommandHandler<UpdateCategoryCommand>
    {
        public async Task HandleAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
        {
            var category = await categoryRepository.GetByIdTrackedAsync(command.Id, cancellationToken);

            if (category is null)
                throw new NotFoundException("Category Not found.");

            var existingCategory = await categoryRepository.GetByNameAsync(command.NewName, cancellationToken);
            if (existingCategory != null && existingCategory.Id != category.Id)
                throw new ConflictException("category already exists");


            category.Rename(command.NewName);
            category.ChangeDescription(command.NewDiscription);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
