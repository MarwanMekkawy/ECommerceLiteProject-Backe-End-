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
    public class EmailChangeTokenRepository(IdentityDbContext _context) : IEmailChangeTokenRepository
    {
        public async Task<EmailChangeToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        {
           return await _context.EmailChangeTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        }

        public async Task AddAsync(EmailChangeToken token, CancellationToken cancellationToken = default)
        {
            await _context.EmailChangeTokens.AddAsync(token, cancellationToken);
        }

        public async Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var emailToken = await _context.EmailChangeTokens.Where(x => x.UserId == userId && x.ConfirmedAt == null).ToListAsync(cancellationToken);
            foreach (var token in emailToken) 
            {
                token.Confirm();
            }
        }

        public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default)
        {
            await _context.EmailChangeTokens.Where(x => x.ExpiresAt <= DateTime.UtcNow).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
