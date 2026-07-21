using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Contracts
{
    public interface IEmailVerificationTokenRepository
    {
        Task<EmailVerificationToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<EmailVerificationToken?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task AddAsync(EmailVerificationToken token, CancellationToken cancellationToken = default);
        Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
    }
}
