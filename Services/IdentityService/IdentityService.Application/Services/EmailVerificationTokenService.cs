using Domain.Exceptions;
using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Exceptions;

namespace IdentityService.Application.Services
{
    public class EmailVerificationTokenService(IUnitOfWork uow, IOneTimeTokenService OTTService ) : IEmailVerificationTokenService
    {
        public async Task<string> GenerateVerificationTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            var token = OTTService.GenerateToken();
            var hashedToken = OTTService.HashToken(token);
            var emailVerificationToken = new EmailVerificationToken() { UserId = userId, TokenHash = hashedToken };
            await uow.emailVerificationTokens.AddAsync(emailVerificationToken, cancellationToken);
            await uow.SaveChangesAsync();
            return token;
        }

        public async Task<string> ResendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken)
        {
            await uow.emailVerificationTokens.InvalidateAllByUserIdAsync(userId, cancellationToken);         
            return await GenerateVerificationTokenAsync(userId, cancellationToken); ;
        }

        public async Task ConfirmEmailAsync(string token, CancellationToken cancellationToken)
        {
            var hashedToken = OTTService.HashToken(token);
            var emailVerificationToken = await uow.emailVerificationTokens.GetByTokenHashAsync(hashedToken, cancellationToken);

            if (emailVerificationToken == null || emailVerificationToken.IsActive == false)
                throw new InvalidTokenException("Invalid or expired verification token.");

            var user = await uow.users.GetByIdAsync(emailVerificationToken.UserId, cancellationToken);
            if (user == null) throw new NotFoundException("the user that you are trying to confirm his email is not found");

            emailVerificationToken.MarkAsVerified();
            user.ConfirmEmail();

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
