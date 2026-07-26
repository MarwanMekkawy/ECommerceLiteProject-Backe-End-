using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Contracts
{
    public interface IEmailChangeTokenRepository
    {
        Task<EmailChangeToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<EmailChangeToken?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task AddAsync(EmailChangeToken token, CancellationToken cancellationToken = default);
        Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
    }
}
