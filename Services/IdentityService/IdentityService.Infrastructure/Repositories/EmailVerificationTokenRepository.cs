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
    public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
    {
        protected readonly IdentityDbContext _context;

        public EmailVerificationTokenRepository(IdentityDbContext context)
        {
            _context = context;
        }
       
        public async Task<EmailVerificationToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        {
            return await _context.EmailVerificationTokens.AsNoTracking().FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        }

        public async Task<EmailVerificationToken?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.EmailVerificationTokens.FirstOrDefaultAsync(x => x.UserId == userId && x.ExpiresAt > DateTime.UtcNow && x.VerifiedAt == null, cancellationToken);
        }


        public async Task AddAsync(EmailVerificationToken token, CancellationToken cancellationToken = default)
        {
            await _context.EmailVerificationTokens.AddAsync(token,cancellationToken);
        }

        public async Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _context.EmailVerificationTokens.Where(x => x.UserId == userId && x.VerifiedAt == null)
                .ExecuteUpdateAsync(t => t.SetProperty(x => x.VerifiedAt, DateTime.UtcNow), cancellationToken);
        }

        public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default)
        {
            await _context.EmailVerificationTokens.Where(x => x.ExpiresAt <= DateTime.UtcNow).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
