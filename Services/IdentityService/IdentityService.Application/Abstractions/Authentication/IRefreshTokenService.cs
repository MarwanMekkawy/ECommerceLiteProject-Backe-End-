using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions.Authentication
{
    public interface IRefreshTokenService
    {
        Task<(RefreshToken StoredToken, string PlaintextToken)> CreateAndStoreRefreshTokenAsync(bool rememberMe, Guid userId, CancellationToken cancellationToken);      
        Task<(RefreshToken StoredToken, string PlaintextToken)?> RotateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken);
    }
}
