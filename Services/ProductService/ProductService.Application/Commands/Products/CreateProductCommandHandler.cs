using Domain.Exceptions;
using ProductService.Application.Abstractions;
using ProductService.Domain.Contracts;
using ProductService.Domain.Entities;


namespace ProductService.Application.Commands.Products
{
    public class CreateProductCommandHandler(IProductRepository productRepository,ICategoryRepository categoryRepository , IUnitOfWork uow) 
        : ICommandHandler<CreateProductCommand, Guid>
    {
        public async Task<Guid> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
        {
            var existingProduct = await productRepository.GetByNameAsync(command.Name, cancellationToken);
            if (existingProduct != null)
                throw new ConflictException("Product already exists");

            var existingCategory = await categoryRepository.ExistsAsync(command.CategoryId, cancellationToken);

            if (!existingCategory)
                throw new NotFoundException("Category was NOT FOUND.");

            var product = new Product(command.Name, command.Description, command.Price, command.StockQuantity, command.CategoryId);

            await productRepository.AddAsync(product, cancellationToken);

            await uow.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
