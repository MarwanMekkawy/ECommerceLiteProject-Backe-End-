using Gateway.API.Abstractions;
using Gateway.API.DTOs;

namespace Gateway.API.GraphQL.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class ProductQueries
    {
        // Public products
        public Task<IReadOnlyList<ProductDto>> GetProducts
            (IProductServiceClient client, int pageNumber = 1, int pageSize = 10, Guid? categoryId = null, CancellationToken cancellationToken = default)
            => client.GetProductsAsync(pageNumber, pageSize, categoryId, cancellationToken);

        public Task<ProductDto?> GetProductById(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.GetProductByIdAsync(id, cancellationToken);

        public Task<ProductDto?> GetProductByName(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.GetProductByNameAsync(name, cancellationToken);

        public Task<IReadOnlyList<ProductDto>> SearchProducts(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.SearchProductsAsync(name, cancellationToken);

        // Admin products
        public Task<IReadOnlyList<ProductWithCategoryDto>> GetAllProductsForAdmin
            (IProductServiceClient client, int pageNumber = 1, int pageSize = 10, Guid? categoryId = null, CancellationToken cancellationToken = default)
            => client.GetAllProductsForAdminAsync(pageNumber, pageSize, categoryId, cancellationToken);

        public Task<ProductDto?> GetProductByIdForAdmin(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.GetProductByIdForAdminAsync(id, cancellationToken);

        public Task<ProductDto?> GetProductByNameForAdmin(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.GetProductByNameForAdminAsync(name, cancellationToken);

        public Task<IReadOnlyList<ProductDto>> SearchProductsForAdmin(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.SearchProductsForAdminAsync(name, cancellationToken);

        // Public categories
        public Task<IReadOnlyList<CategoryDto>> GetCategories
            (IProductServiceClient client, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
            => client.GetCategoriesAsync(pageNumber, pageSize, cancellationToken);

        public Task<CategoryDto?> GetCategoryById(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.GetCategoryByIdAsync(id, cancellationToken);

        public Task<CategoryDto?> GetCategoryByName(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.GetCategoryByNameAsync(name, cancellationToken);

        public Task<IReadOnlyList<CategoryDto>> SearchCategories(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.SearchCategoriesAsync(name, cancellationToken);

        // Admin categories
        public Task<IReadOnlyList<CategoryDto>> GetAllCategoriesForAdmin
            (IProductServiceClient client, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
            => client.GetAllCategoriesForAdminAsync(pageNumber, pageSize, cancellationToken);

        public Task<CategoryDto?> GetCategoryByIdForAdmin(IProductServiceClient client, Guid id, CancellationToken cancellationToken)
            => client.GetCategoryByIdForAdminAsync(id, cancellationToken);

        public Task<CategoryDto?> GetCategoryByNameForAdmin(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.GetCategoryByNameForAdminAsync(name, cancellationToken);

        public Task<IReadOnlyList<CategoryDto>> SearchCategoriesForAdmin(IProductServiceClient client, string name, CancellationToken cancellationToken)
            => client.SearchCategoriesForAdminAsync(name, cancellationToken);
    }
}
