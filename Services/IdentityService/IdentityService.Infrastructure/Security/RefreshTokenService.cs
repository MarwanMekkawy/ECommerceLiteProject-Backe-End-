using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Security
{
    public class RefreshTokenService(IConfiguration Configuration, IUnitOfWork uow) : IRefreshTokenService
    {      
        //[helper methods]===================================================================
        private string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes);
        }
        private string HashRefreshToken(string token)
        {
            var secret = Configuration["RefreshToken:Secret"]!;
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(token)));
        }
        //===================================================================================

        public async Task<(RefreshToken StoredToken, string PlaintextToken)> CreateAndStoreRefreshTokenAsync(Guid userId)
        {
            var rawToken = GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(Configuration["RefreshToken:ExpiryInMinutes"]!)),
                TokenHash = HashRefreshToken(rawToken)
            };

            await uow.refreshTokens.AddAsync(refreshToken);

            return (refreshToken, rawToken);
        }


        public async Task<RefreshToken?> ValidateRefreshTokenAsync(string refreshToken)
        {
            var hash = HashRefreshToken(refreshToken);
            return await uow.refreshTokens.GetByTokenHashAsync(hash);
        }


        public async Task<(RefreshToken StoredToken, string PlaintextToken)?> RotateRefreshTokenAsync(string refreshToken)
        {
            var hash = HashRefreshToken(refreshToken);
            var existingToken = await uow.refreshTokens.GetByTokenHashAsync(hash);

            if (existingToken == null)
                throw new InvalidTokenException("Refresh token not recognized.");

            var userId = existingToken.UserId;

            // reusing invalid token [not Atctive token - old token that is replaced]
            if (!existingToken.IsActive)
            {
                if (existingToken.ReplacedByTokenHash != null)
                {
                    // the use of old token that is replaced
                    await uow.refreshTokens.RevokeAllByUserIdAsync(userId);
                    return null;                    
                }
                throw new InvalidTokenException("Refresh token expired or revoked.");
            }

            var newRawToken = GenerateRefreshToken();
            var newHash = HashRefreshToken(newRawToken);

            var newToken = new RefreshToken
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(Configuration["RefreshToken:ExpiryInMinutes"]!)),
                TokenHash = newHash
            };

            existingToken.Revoke(newToken.TokenHash);
            await uow.refreshTokens.AddAsync(newToken);

            return (newToken, newRawToken);
        }


        public async Task RevokeAllUserRefreshTokensAsync(Guid userId)
        {
            await uow.refreshTokens.RevokeAllByUserIdAsync(userId);
        }


        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var hashedRefreshToken = HashRefreshToken(refreshToken);
            await uow.refreshTokens.RevokeAsync(hashedRefreshToken);
        }      
    }
}
