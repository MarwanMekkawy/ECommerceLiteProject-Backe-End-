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
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToke);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);

        Task<IReadOnlyList<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<IReadOnlyList<User>> GetPagedActiveAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<IReadOnlyList<User>> GetPagedInactiveAsync(int page, int pageSize, CancellationToken cancellationToken);

        Task<int> GetTotalCountAsync(CancellationToken cancellationToken);
        Task<int> GetActiveCountAsync(CancellationToken cancellationToke);

        Task AddAsync(User user, CancellationToken cancellationToken);
        Task ActivateAsync(Guid id, CancellationToken cancellationToken);
        Task DeactivateAsync(Guid id, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
