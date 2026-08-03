using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Commands.Products
{
    public class IncreaseStockCommandHandler(IProductRepository productRepository, IUnitOfWork uow) : ICommandHandler<IncreaseStockCommand, int>
    {
        public async Task<int> HandleAsync(IncreaseStockCommand command, CancellationToken cancellationToken = default)
        {
            var existingProduct = await productRepository.GetByIdTrackedAsync(command.ProductId, cancellationToken);

            if (existingProduct is null)
                throw new NotFoundException("Product was NOT FOUND.");

            existingProduct.IncreaseStock(command.Quantity);

            await uow.SaveChangesAsync(cancellationToken);

            return existingProduct.StockQuantity;
        }
    }
}
