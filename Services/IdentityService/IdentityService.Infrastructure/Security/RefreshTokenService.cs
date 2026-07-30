using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;


namespace IdentityService.Infrastructure.Security
{
    public class RefreshTokenService(IConfiguration configuration, IUnitOfWork uow) : IRefreshTokenService
    {
        #region//[helper methods]===================================================================
        private string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes);
        }
        private string HashRefreshToken(string token)
        {
            var secret = configuration["RefreshToken:Secret"]!;
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(token)));
        }
        #endregion ===================================================================================

        public async Task<(RefreshToken StoredToken, string PlaintextToken)> CreateAndStoreRefreshTokenAsync(bool rememberMe, Guid userId, CancellationToken cancellationToken)
        {
            var rawToken = GenerateRefreshToken();

            var expiryMinutes = rememberMe ? configuration["RefreshToken:ExpiryInMinutesRememberMe"] : configuration["RefreshToken:ExpiryInMinutes"];

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                RememberMe = rememberMe,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(expiryMinutes!)),
                TokenHash = HashRefreshToken(rawToken)
            };

            await uow.refreshTokens.AddAsync(refreshToken, cancellationToken);

            return (refreshToken, rawToken);
        }

        public async Task<(RefreshToken StoredToken, string PlaintextToken)?> RotateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var hash = HashRefreshToken(refreshToken);
            var existingToken = await uow.refreshTokens.GetByTokenHashAsync(hash, cancellationToken);

            if (existingToken == null)
                throw new InvalidTokenException("Refresh token not recognized.");

            var userId = existingToken.UserId;

            // reusing invalid token [not Atctive token - old token that is replaced]
            if (!existingToken.IsActive)
            {
                if (existingToken.ReplacedByTokenHash != null)
                {
                    // the use of old token that is replaced
                    await RevokeAllUserRefreshTokensAsync(userId, cancellationToken);
                    return null;                    
                }
                throw new InvalidTokenException("Refresh token expired or revoked.");
            }

            var newRawToken = GenerateRefreshToken();
            var newHash = HashRefreshToken(newRawToken);

            var expiryMinutes = existingToken.RememberMe ? configuration["RefreshToken:ExpiryInMinutesRememberMe"] : configuration["RefreshToken:ExpiryInMinutes"];

            var newToken = new RefreshToken
            {
                UserId = userId,
                RememberMe = existingToken.RememberMe,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(expiryMinutes!)),
                TokenHash = newHash
            };

            existingToken.Revoke(newToken.TokenHash);
            await uow.refreshTokens.AddAsync(newToken, cancellationToken);

            return (newToken, newRawToken);
        }


        public async Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
        {
            var tokens = await uow.refreshTokens.GetActiveByUserIdAsync(userId, cancellationToken);

            foreach (var token in tokens)
            {
                token.Revoke();
            }

            await uow.SaveChangesAsync(cancellationToken);
        }


        public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var hashedRefreshToken = HashRefreshToken(refreshToken);

            var token = await uow.refreshTokens.GetByTokenHashAsync(hashedRefreshToken, cancellationToken) 
                ?? throw new InvalidTokenException("Refresh token not recognized.");

            token.Revoke();

            await uow.SaveChangesAsync(cancellationToken);
        }      
    }
}
