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
    public class PasswordResetTokenRepository(IdentityDbContext _context) : IPasswordResetTokenRepository
    {    
        public async Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        {
            return await _context.PasswordResetTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        }

        public async Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default)
        {
            await _context.PasswordResetTokens.AddAsync(token, cancellationToken);
        }

        public async Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var passwordTokens = await _context.PasswordResetTokens.Where(x => x.UserId == userId && x.UsedAt == null).ToListAsync(cancellationToken);
                foreach (var token in passwordTokens) 
                {
                    token.MarkAsUsed();
                }
        }

        public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default)
        {
            await _context.PasswordResetTokens.Where(x => x.ExpiresAt <= DateTime.UtcNow).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
