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

        public async Task<ProductMutationPayload> UpdateProduct
            (IProductServiceClient client, Guid id, UpdateProductRequestDto dto, CancellationToken cancellationToken)
        {
            await client.UpdateProductAsync(id, dto, cancellationToken);
            return new ProductMutationPayload(true);
        }
        public async Task<ProductMutationPayload> DeleteProduct(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
        {
            await client.DeleteProductAsync(id, cancellationToken);
            return new ProductMutationPayload(true);
        }
        public async Task<ProductMutationPayload> ActivateProduct(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
        {
            await client.ActivateProductAsync(id, cancellationToken);
            return new ProductMutationPayload(true);
        }
        public async Task<ProductMutationPayload> DeactivateProduct(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
        {
            await client.DeactivateProductAsync(id, cancellationToken);
            return new ProductMutationPayload(true);
        }
        public async Task<ProductMutationPayload> IncreaseStock(IProductServiceClient client, Guid id, int quantity, CancellationToken cancellationToken)
        {
            await client.IncreaseStockAsync(id, quantity, cancellationToken);
            return new ProductMutationPayload(true);
        }
        public async Task<ProductMutationPayload> DecreaseStock(IProductServiceClient client, Guid id, int quantity, CancellationToken cancellationToken)
        {
            await client.DecreaseStockAsync(id, quantity, cancellationToken);
            return new ProductMutationPayload(true);
        }

        // Categories
        public Task<Guid> CreateCategory
            (IProductServiceClient client, CreateCategoryRequestDto dto, CancellationToken cancellationToken)
            => client.CreateCategoryAsync(dto, cancellationToken);

        public async Task<ProductMutationPayload> UpdateCategory(IProductServiceClient client, Guid id, UpdateCategoryRequestDto dto, CancellationToken cancellationToken) 
        { 
            await client.UpdateCategoryAsync(id, dto, cancellationToken); 
            return new ProductMutationPayload(true); 
        }
        public async Task<ProductMutationPayload> DeleteCategory(IProductServiceClient client, Guid id, CancellationToken cancellationToken) 
        { 
            await client.DeleteCategoryAsync(id, cancellationToken); 
            return new ProductMutationPayload(true); 
        }
        public async Task<ProductMutationPayload> ActivateCategory(IProductServiceClient client, Guid id, CancellationToken cancellationToken) 
        { 
            await client.ActivateCategoryAsync(id, cancellationToken); 
            return new ProductMutationPayload(true); 
        }
        public async Task<ProductMutationPayload> DeactivateCategory(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
        {
            await client.DeactivateCategoryAsync(id, cancellationToken); 
            return new ProductMutationPayload(true);
        }
    }
    public sealed record ProductMutationPayload(bool Success);
}
