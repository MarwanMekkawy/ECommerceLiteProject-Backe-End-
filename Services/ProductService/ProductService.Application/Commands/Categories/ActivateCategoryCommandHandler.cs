using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Commands.Categories
{
    public class ActivateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork uow) : ICommandHandler<ActivateCategoryCommand>
    {
        public async Task HandleAsync(ActivateCategoryCommand command, CancellationToken cancellationToken = default)
        {
            var existingCategory = await categoryRepository.GetByIdTrackedAsync(command.CategoryId, cancellationToken);
            if (existingCategory is null)
                throw new NotFoundException("Category was NOT FOUND.");

            existingCategory.Activate();

            await uow.SaveChangesAsync(cancellationToken);  
        }
    }
}
