using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Commands.Products
{
    public class UpdateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork uow)
        : ICommandHandler<UpdateProductCommand>
    {
        public async Task HandleAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
        {
            var existingProduct = await productRepository.GetByIdTrackedAsync(command.ProductId, cancellationToken);
            if (existingProduct is null)
                throw new NotFoundException("Product was NOT FOUND.");

            var existingCategory = await categoryRepository.ExistsAsync(command.NewCategoryId, cancellationToken);
            if(!existingCategory)
                throw new NotFoundException("Category was NOT FOUND.");

            var existingSameName = await productRepository.GetByNameAsync(command.NewName, cancellationToken);
            if (existingSameName != null && existingSameName.Id != existingProduct.Id)
                throw new ConflictException("Product already exists");

            existingProduct.Update(command.NewName, command.NewDescription, command.NewPrice, command.NewCategoryId);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
