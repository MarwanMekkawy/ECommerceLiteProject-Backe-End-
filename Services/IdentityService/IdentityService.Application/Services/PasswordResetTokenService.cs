using Domain.Exceptions;
using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Application.DTOs.PwResetDTOs;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Services
{
    public class PasswordResetTokenService(IUnitOfWork uow, IOneTimeTokenService OTTService, IPasswordHasher hasher) : IPasswordResetService
    {
        #region //[helper methods]========================================================
        private bool IsStrongPassword(string password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
        }
        private void ValidatePassword(string Password, string ConfirmPassword)
        {
            if (string.IsNullOrWhiteSpace(Password)) throw new BadRequestException("Password Cant be empty");
            if (Password != ConfirmPassword) throw new BadRequestException("New password confirmation must match the password");
            if (Password.Length < 8) throw new BadRequestException("New password must be at least 8 characters");
            if (!IsStrongPassword(Password)) throw new BadRequestException("Password is too weak");
        }
        private string NormalizeEmail(string email)
        {
            return email?.Trim().ToLowerInvariant() ?? string.Empty;
        }
        #endregion//========================================================================

        public async Task<string> RequestPasswordResetAsync(ForgotPasswordDto dto, CancellationToken cancellationToken)
        {
            var token = OTTService.GenerateToken();
            var hashedToken = OTTService.HashToken(token);
            var normalizedEmail = NormalizeEmail(dto.Email);

            var user = await uow.users.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (user == null)
                return string.Empty;

            var passwordResetToken = new PasswordResetToken() { UserId = user.Id, TokenHash = hashedToken };

            await uow.passwordResetTokens.InvalidateAllByUserIdAsync(user.Id, cancellationToken);
            await uow.passwordResetTokens.AddAsync(passwordResetToken, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            return token;
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            var hashedToken = OTTService.HashToken(dto.Token);
            var oldPassword = dto.OldPassword;
            var newPassword = dto.NewPassword;
            var confirmPassword = dto.ConfirmPassword;

            ValidatePassword(newPassword, confirmPassword);
            var newPasswordHash = hasher.Hash(newPassword);

            var passwordResetToken = await uow.passwordResetTokens.GetByTokenHashAsync(hashedToken, cancellationToken);
            if (passwordResetToken == null || passwordResetToken.IsActive == false)
                throw new InvalidTokenException("Invalid or expired verification token.");

            var user = await uow.users.GetByIdAsync(passwordResetToken.UserId, cancellationToken);
            if (user == null)
                throw new NotFoundException("the user that you are trying to change password for is not found");

            var isOldPasswordValid = hasher.Verify(user.PasswordHash, oldPassword);
            if (!isOldPasswordValid)
                throw new UnauthorizedException("your Old password is Wrong");
            

            passwordResetToken.MarkAsUsed();
            user.ChangePassword(newPasswordHash);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
