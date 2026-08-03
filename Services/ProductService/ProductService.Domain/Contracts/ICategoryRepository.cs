using ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Contracts
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Category?> GetByIdUntrackedAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Category>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Category>> GetPaginatedUntrackedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(Category category, CancellationToken cancellationToken = default);

        void Remove(Category category);
    }
}
