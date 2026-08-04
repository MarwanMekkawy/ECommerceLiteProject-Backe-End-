using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Commands.Products
{
    public class DeactivateProductCommandHandler(IProductRepository productRepository, IUnitOfWork uow) : ICommandHandler<DeactivateProductCommand>
    {
        public async Task HandleAsync(DeactivateProductCommand command, CancellationToken cancellationToken = default)
        {
            var existingProduct = await productRepository.GetByIdTrackedAsync(command.ProductId, cancellationToken);
            if (existingProduct is null)
                throw new NotFoundException("Product was NOT FOUND.");

            existingProduct.Deactivate();

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}

