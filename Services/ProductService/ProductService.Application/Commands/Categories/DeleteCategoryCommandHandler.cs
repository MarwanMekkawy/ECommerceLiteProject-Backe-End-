using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Commands.Categories
{
    public class DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork uow) : ICommandHandler<DeleteCategoryCommand>
    {
        public async Task HandleAsync(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
        {
            var category = await categoryRepository.GetByIdTrackedAsync(command.Id, cancellationToken);

            if (category is null)
                throw new NotFoundException("Category Not found.");

            if (await categoryRepository.HasProductsAsync(command.Id, cancellationToken))
                throw new ConflictException("Cannot delete a category that contains products. Deactivate it instead.");

            categoryRepository.Remove(category);

            await uow.SaveChangesAsync(cancellationToken); 
        }
    }
}
