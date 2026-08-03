using ProductService.Domain.Entities;


namespace ProductService.Domain.Contracts
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Product?> GetByIdUntrackedAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> GetPaginatedUntrackedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task AddAsync(Product product, CancellationToken cancellationToken = default);

        void Remove(Product product);
    }
}
