using Gateway.API.Abstractions;
using Gateway.API.DTOs;

namespace Gateway.API.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class ProductMutations
    {
        // Products
        public Task<Guid> CreateProduct
            (IProductServiceClient client, CreateProductRequestDto dto, CancellationToken cancellationToken)
            => client.CreateProductAsync(dto, cancellationToken);

        public Task UpdateProduct
            (IProductServiceClient client, Guid id, UpdateProductRequestDto dto, CancellationToken cancellationToken)
            => client.UpdateProductAsync(id, dto, cancellationToken);

        public Task DeleteProduct(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.DeleteProductAsync(id, cancellationToken);

        public Task ActivateProduct(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.ActivateProductAsync(id, cancellationToken);

        public Task DeactivateProduct(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.DeactivateProductAsync(id, cancellationToken);

        public Task IncreaseStock(IProductServiceClient client, Guid id, int quantity, CancellationToken cancellationToken)
            => client.IncreaseStockAsync(id, quantity, cancellationToken);

        public Task DecreaseStock(IProductServiceClient client, Guid id, int quantity, CancellationToken cancellationToken)
            => client.DecreaseStockAsync(id, quantity, cancellationToken);

        // Categories
        public Task<Guid> CreateCategory
            (IProductServiceClient client, CreateCategoryRequestDto dto, CancellationToken cancellationToken)
            => client.CreateCategoryAsync(dto, cancellationToken);

        public Task UpdateCategory
            (IProductServiceClient client, Guid id, UpdateCategoryRequestDto dto, CancellationToken cancellationToken)
            => client.UpdateCategoryAsync(id, dto, cancellationToken);

        public Task DeleteCategory(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.DeleteCategoryAsync(id, cancellationToken);

        public Task ActivateCategory(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.ActivateCategoryAsync(id, cancellationToken);

        public Task DeactivateCategory(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.DeactivateCategoryAsync(id, cancellationToken);
    }
}
