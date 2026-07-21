using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Contracts
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<RefreshToken?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task RevokeAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
    }
}
