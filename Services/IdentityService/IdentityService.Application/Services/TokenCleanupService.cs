using IdentityService.Application.Abstractions;
using IdentityService.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Services
{
    public class TokenCleanupService(IUnitOfWork uow) : ITokenCleanupService
    {
        public async Task CleanupAsync(CancellationToken cancellationToken)
        {
            await uow.emailChangeTokens.DeleteExpiredAsync(cancellationToken);  
            await uow.emailVerificationTokens.DeleteExpiredAsync(cancellationToken);
            await uow.passwordResetTokens.DeleteExpiredAsync(cancellationToken);
            await uow.refreshTokens.DeleteExpiredAsync(cancellationToken);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
