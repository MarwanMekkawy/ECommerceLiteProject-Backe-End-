using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Contracts
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetPagedActiveAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetPagedInactiveAsync(int page, int pageSize, CancellationToken cancellationToken = default);

        Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
        Task<int> GetActiveCountAsync(CancellationToken cancellationToken = default);

        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
