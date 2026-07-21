using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Contracts
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<PasswordResetToken?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default);
        Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
    }
}
