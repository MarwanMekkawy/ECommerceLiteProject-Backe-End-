using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Commands.Products
{
    public class ActivateProductCommandHandler(IProductRepository productRepository, IUnitOfWork uow) : ICommandHandler<ActivateProductCommand>
    {
        public async Task HandleAsync(ActivateProductCommand command, CancellationToken cancellationToken = default)
        {
            var existingProduct = await productRepository.GetByIdTrackedAsync(command.ProductId, cancellationToken);
            if (existingProduct is null)
                throw new NotFoundException("Product was NOT FOUND.");

            existingProduct.Activate();

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
