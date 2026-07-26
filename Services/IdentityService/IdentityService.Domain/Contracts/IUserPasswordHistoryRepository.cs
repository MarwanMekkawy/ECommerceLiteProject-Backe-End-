using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Contracts
{
    public interface IUserPasswordHistoryRepository
    {
        Task AddAsync(UserPasswordHistory historyRecord, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<UserPasswordHistory>> GetAllByUserIdAsync(Guid userId, int count, CancellationToken cancellationToken = default);
        void Delete(UserPasswordHistory oldPassword);
    }
}
