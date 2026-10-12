using Gateway.API.DTOs;

namespace Gateway.API.Abstractions
{
    public interface IProductServiceClient
    {
        // Public categories
        Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<CategoryDto?> GetCategoryByNameAsync(string name, CancellationToken cancellationToken);
        Task<IReadOnlyList<CategoryDto>> SearchCategoriesAsync(string name, CancellationToken cancellationToken);
        // Admin categories
        Task<IReadOnlyList<CategoryDto>> GetAllCategoriesForAdminAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<CategoryDto?> GetCategoryByIdForAdminAsync(Guid id, CancellationToken cancellationToken);
        Task<CategoryDto?> GetCategoryByNameForAdminAsync(string name, CancellationToken cancellationToken);
        Task<IReadOnlyList<CategoryDto>> SearchCategoriesForAdminAsync(string name, CancellationToken cancellationToken);
        Task<Guid> CreateCategoryAsync(CreateCategoryRequestDto dto, CancellationToken cancellationToken);
        Task UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto dto, CancellationToken cancellationToken);
        Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken);
        Task ActivateCategoryAsync(Guid id, CancellationToken cancellationToken);
        Task DeactivateCategoryAsync(Guid id, CancellationToken cancellationToken);

        // Public products
        Task<IReadOnlyList<ProductDto>> GetProductsAsync(int pageNumber, int pageSize, Guid? categoryId, CancellationToken cancellationToken);
        Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<ProductDto?> GetProductByNameAsync(string name, CancellationToken cancellationToken);
        Task<IReadOnlyList<ProductDto>> SearchProductsAsync(string name, CancellationToken cancellationToken);
        // Admin products
        Task<Guid> CreateProductAsync(CreateProductRequestDto dto, CancellationToken cancellationToken);
        Task UpdateProductAsync(Guid id, UpdateProductRequestDto dto, CancellationToken cancellationToken);
        Task DeleteProductAsync(Guid id, CancellationToken cancellationToken);
        Task ActivateProductAsync(Guid id, CancellationToken cancellationToken);
        Task DeactivateProductAsync(Guid id, CancellationToken cancellationToken);
        Task IncreaseStockAsync(Guid id, int quantity, CancellationToken cancellationToken);
        Task DecreaseStockAsync(Guid id, int quantity, CancellationToken cancellationToken);
        Task<IReadOnlyList<ProductDto>> GetAllProductsForAdminAsync(int pageNumber, int pageSize, Guid? categoryId, CancellationToken cancellationToken);
        Task<ProductDto?> GetProductByIdForAdminAsync(Guid id, CancellationToken cancellationToken);
        Task<ProductDto?> GetProductByNameForAdminAsync(string name, CancellationToken cancellationToken);
        Task<IReadOnlyList<ProductDto>> SearchProductsForAdminAsync(string name, CancellationToken cancellationToken);
    }
}
