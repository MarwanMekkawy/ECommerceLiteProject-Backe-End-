using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Contracts
{
    public interface IUnitOfWork
    {
        public IUserRepository users { get; }
        public IRefreshTokenRepository refreshTokens { get; }
        public IEmailVerificationTokenRepository emailVerificationTokens { get; }
        public IPasswordResetTokenRepository passwordResetTokens  { get; }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
