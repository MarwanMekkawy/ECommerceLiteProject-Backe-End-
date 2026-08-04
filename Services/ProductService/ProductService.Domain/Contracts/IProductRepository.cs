using ProductService.Domain.Entities;


namespace ProductService.Domain.Contracts
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Product?> GetByIdTrackedWithCategoryAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Product?> GetByIdUntrackedAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> SearchByNameIncludeInactiveAsync(string searchTerm, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> GetPaginatedUntrackedAsync
            (int pageNumber, int pageSize, Guid? categoryId, bool includeInactives, CancellationToken cancellationToken = default);

        Task AddAsync(Product product, CancellationToken cancellationToken = default);

        void Remove(Product product);
    }
}
