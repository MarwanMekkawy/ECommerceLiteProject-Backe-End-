using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Commands.Categories
{
    public class DeactivateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork uow) : ICommandHandler<DeactivateCategoryCommand>
    {
        public async Task HandleAsync(DeactivateCategoryCommand command, CancellationToken cancellationToken = default)
        {
            var existingCategory = await categoryRepository.GetByIdTrackedAsync(command.CategoryId, cancellationToken);
            if (existingCategory is null)
                throw new NotFoundException("Category was NOT FOUND.");

            existingCategory.Deactivate();

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
