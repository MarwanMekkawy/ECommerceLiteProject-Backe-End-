using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Commands.Products
{
    public class ActivateProductCommandHandler(IProductRepository productRepository, IUnitOfWork uow) : ICommandHandler<ActivateProductCommand>
    {
        public async Task HandleAsync(ActivateProductCommand command, CancellationToken cancellationToken = default)
        {
            var existingProduct = await productRepository.GetByIdTrackedWithCategoryAsync(command.ProductId, cancellationToken);
            if (existingProduct is null)
                throw new NotFoundException("Product was NOT FOUND.");

            if (!existingProduct.Category.IsActive)
                throw new BadRequestException("The product category is inactive");

            existingProduct.Activate();

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
