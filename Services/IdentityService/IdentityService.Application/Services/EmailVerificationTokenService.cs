using Domain.Exceptions;
using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Application.DTOs.EmailVerificationDTOs;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Exceptions;
using System.Net.Mail;

namespace IdentityService.Application.Services
{
    public class EmailVerificationTokenService(IUnitOfWork uow, IOneTimeTokenService OTTService ,IPasswordHasher hasher) : IEmailVerificationTokenService
    {
        #region //[helper methods]========================================================      
        private string NormalizeEmail(string email)
        {
            return email?.Trim().ToLowerInvariant() ?? string.Empty;
        }
        private bool IsValidEmail(string email)
        {
            return MailAddress.TryCreate(email, out _);
        }    
        #endregion//========================================================================

        public async Task<GenerateVerificationEmailDto> GenerateVerificationTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            var token = OTTService.GenerateToken();
            var hashedToken = OTTService.HashToken(token);
            var emailVerificationToken = new EmailVerificationToken() { UserId = userId, TokenHash = hashedToken };
            var user = await uow.users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found");
         
            await uow.emailVerificationTokens.AddAsync(emailVerificationToken, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            return new GenerateVerificationEmailDto() { Email = user.Email, Token = token };
        }

        // overload as helper for the resend
        private async Task<GenerateVerificationEmailDto> GenerateVerificationTokenAsync(User user, CancellationToken cancellationToken)
        {
            var token = OTTService.GenerateToken();
            var hashedToken = OTTService.HashToken(token);
            var emailVerificationToken = new EmailVerificationToken() { UserId = user.Id, TokenHash = hashedToken };            

            await uow.emailVerificationTokens.AddAsync(emailVerificationToken, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            return new GenerateVerificationEmailDto() { Email = user.Email, Token = token };
        }

        public async Task<GenerateVerificationEmailDto> ResendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await uow.users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found");

            if (!user.CanResendVerificationEmail())
            {
                var cd = user.ResendCooldownSeconds();
                throw new TooManyRequestsException($"Please wait {cd}s before requesting another email.");
            }

            user.StartVerificationEmailCooldown();

            await uow.emailVerificationTokens.InvalidateAllByUserIdAsync(userId, cancellationToken);         
            return await GenerateVerificationTokenAsync(user, cancellationToken);
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

        //Email Change
        public async Task<string> GenerateEmailChangeTokenAsync(Guid userId, ChangeEmailRequestDto dto, CancellationToken cancellationToken)
        {
            var token = OTTService.GenerateToken();
            var hashedToken = OTTService.HashToken(token);
            var providedPassword = dto.Password;
            var normalizedNewEmail = NormalizeEmail(dto.NewEmail);

            if(!IsValidEmail(normalizedNewEmail))
                throw new BadRequestException("Invalid email format");

            var existingEmail= await uow.users.ExistsByEmailAsync(normalizedNewEmail, cancellationToken);
            if(existingEmail)
                throw new BadRequestException("email already in use");

            var user = await uow.users.GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new NotFoundException("couldnt find the user");

            if(user.Email == normalizedNewEmail)
                throw new BadRequestException("the new email cant be same as your current email");

            var userPasswordHash = user.PasswordHash;

            if(!hasher.Verify(userPasswordHash, providedPassword))
                throw new UnauthorizedException("the password is Wrong");

            var emailChangeToken = new EmailChangeToken() { UserId = userId, NewEmail = normalizedNewEmail, TokenHash=hashedToken };

            await uow.emailChangeTokens.InvalidateAllByUserIdAsync(userId, cancellationToken);
            await uow.emailChangeTokens.AddAsync(emailChangeToken, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            return token;
        }

        public async Task ConfirmEmailChangeAsync(string token, CancellationToken cancellationToken)
        {
            var hashedToken = OTTService.HashToken(token);

            var emailChangeToken = await uow.emailChangeTokens.GetByTokenHashAsync(hashedToken, cancellationToken);
            if(emailChangeToken == null || emailChangeToken.IsActive == false)
                throw new InvalidTokenException("Invalid or expired verification token.");

            var existingEmail = await uow.users.ExistsByEmailAsync(emailChangeToken.NewEmail, cancellationToken);
            if (existingEmail)
                throw new BadRequestException("email already in use");

            var user = await uow.users.GetByIdAsync(emailChangeToken.UserId, cancellationToken);
            if (user == null)
                throw new NotFoundException("the user that you are trying to change email for is not found");


            emailChangeToken.Confirm();
            user.ChangeEmail(emailChangeToken.NewEmail);
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
