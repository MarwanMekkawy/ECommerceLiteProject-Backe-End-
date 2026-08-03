using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;


namespace ProductService.Application.Commands.Products
{
    public class DecreaseStockCommandHandler(IProductRepository productRepository, IUnitOfWork uow) : ICommandHandler<DecreaseStockCommand, int>
    {
        public async Task<int> HandleAsync(DecreaseStockCommand command, CancellationToken cancellationToken = default)
        {
            var existingProduct = await productRepository.GetByIdTrackedAsync(command.ProductId, cancellationToken);

            if (existingProduct is null) 
                    throw new NotFoundException("Product was NOT FOUND.");

            existingProduct.DecreaseStock(command.Quantity);

            await uow.SaveChangesAsync(cancellationToken);

            return existingProduct.StockQuantity;
        }
    }
}
