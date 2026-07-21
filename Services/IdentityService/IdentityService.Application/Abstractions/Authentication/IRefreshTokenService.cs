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
        Task<(RefreshToken StoredToken, string PlaintextToken)> CreateAndStoreRefreshTokenAsync(Guid userId);
        Task<RefreshToken?> ValidateRefreshTokenAsync(string refreshToken);
        Task<(RefreshToken StoredToken, string PlaintextToken)?> RotateRefreshTokenAsync(string refreshToken, Guid userId);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task RevokeAllUserRefreshTokensAsync(Guid userId);
    }
}
