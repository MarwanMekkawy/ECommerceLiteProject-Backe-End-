using Domain.Exceptions;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        protected readonly IdentityDbContext _context;

        public RefreshTokenRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        }

        public async Task<RefreshToken?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId && x.ExpiresAt > DateTime.UtcNow && x.RevokedAt == null, cancellationToken);
        }


        public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        }

        public async Task RevokeAsync(string tokenHash, CancellationToken cancellationToken = default)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken) ?? throw new NotFoundException();
            token.Revoke();
        }

        public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var tokens = await _context.RefreshTokens.Where(t => t.UserId == userId && t.RevokedAt == null).ToListAsync(cancellationToken);

            foreach (var token in tokens)
                token.Revoke();
        }

        public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var revokedRetentionCutoff = now.AddDays(-30); // keeping last month revoked tokens

            await _context.RefreshTokens
                .Where(x => x.ExpiresAt <= now || (x.RevokedAt != null && x.RevokedAt <= revokedRetentionCutoff))
                .ExecuteDeleteAsync(cancellationToken);
        } 
    }
}
