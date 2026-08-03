using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Commands.Products
{
    public class DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork uow) : ICommandHandler<DeleteProductCommand>
    {
        public async Task HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
        {
            var existingProduct = await productRepository.GetByIdUntrackedAsync(command.ProductId, cancellationToken);

            if (existingProduct is null)
                throw new NotFoundException("Product was NOT FOUND.");

            productRepository.Remove(existingProduct);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
