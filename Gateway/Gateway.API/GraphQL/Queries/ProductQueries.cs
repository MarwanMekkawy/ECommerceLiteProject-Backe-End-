using Gateway.API.Abstractions;
using Gateway.API.DTOs;
using HotChocolate.Authorization;

namespace Gateway.API.GraphQL.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class ProductQueries
    {
        // Public products
        [AllowAnonymous]
        public Task<IReadOnlyList<ProductDto>> GetProducts
            (IProductServiceClient client, int pageNumber = 1, int pageSize = 10, Guid? categoryId = null, CancellationToken cancellationToken = default)
            => client.GetProductsAsync(pageNumber, pageSize, categoryId, cancellationToken);
        [AllowAnonymous]
        public Task<ProductDto?> GetProductById(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.GetProductByIdAsync(id, cancellationToken);
        [AllowAnonymous]
        public Task<ProductDto?> GetProductByName(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.GetProductByNameAsync(name, cancellationToken);
        [AllowAnonymous]
        public Task<IReadOnlyList<ProductDto>> SearchProducts(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.SearchProductsAsync(name, cancellationToken);

        // Admin products
        [Authorize(Roles = ["Admin"])]
        public Task<IReadOnlyList<ProductWithCategoryDto>> GetAllProductsForAdmin
            (IProductServiceClient client, int pageNumber = 1, int pageSize = 10, Guid? categoryId = null, CancellationToken cancellationToken = default)
            => client.GetAllProductsForAdminAsync(pageNumber, pageSize, categoryId, cancellationToken);
        [Authorize(Roles = ["Admin"])]
        public Task<ProductDto?> GetProductByIdForAdmin(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.GetProductByIdForAdminAsync(id, cancellationToken);
        [Authorize(Roles = ["Admin"])]
        public Task<ProductDto?> GetProductByNameForAdmin(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.GetProductByNameForAdminAsync(name, cancellationToken);
        [Authorize(Roles = ["Admin"])]
        public Task<IReadOnlyList<ProductDto>> SearchProductsForAdmin(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.SearchProductsForAdminAsync(name, cancellationToken);

        // Public categories
        [AllowAnonymous]
        public Task<IReadOnlyList<CategoryDto>> GetCategories
            (IProductServiceClient client, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
            => client.GetCategoriesAsync(pageNumber, pageSize, cancellationToken);
        [AllowAnonymous]
        public Task<CategoryDto?> GetCategoryById(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.GetCategoryByIdAsync(id, cancellationToken);
        [AllowAnonymous]
        public Task<CategoryDto?> GetCategoryByName(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.GetCategoryByNameAsync(name, cancellationToken);
        [AllowAnonymous]
        public Task<IReadOnlyList<CategoryDto>> SearchCategories(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.SearchCategoriesAsync(name, cancellationToken);

        // Admin categories
        [Authorize(Roles = ["Admin"])]
        public Task<IReadOnlyList<CategoryDto>> GetAllCategoriesForAdmin
            (IProductServiceClient client, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
            => client.GetAllCategoriesForAdminAsync(pageNumber, pageSize, cancellationToken);
        [Authorize(Roles = ["Admin"])]
        public Task<CategoryDto?> GetCategoryByIdForAdmin(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.GetCategoryByIdForAdminAsync(id, cancellationToken);
        [Authorize(Roles = ["Admin"])]
        public Task<CategoryDto?> GetCategoryByNameForAdmin(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.GetCategoryByNameForAdminAsync(name, cancellationToken);
        [Authorize(Roles = ["Admin"])]
        public Task<IReadOnlyList<CategoryDto>> SearchCategoriesForAdmin(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.SearchCategoriesForAdminAsync(name, cancellationToken);
    }
}
