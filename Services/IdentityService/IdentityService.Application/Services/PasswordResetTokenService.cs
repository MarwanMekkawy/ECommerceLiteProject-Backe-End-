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
    public class PasswordResetTokenService(IUnitOfWork uow, IOneTimeTokenService OTTService, IPasswordHasher hasher) : IPasswordResetTokenService
    {
        #region //[helper methods]========================================================
        private bool IsStrongPassword(string password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
        }
        private void ValidatePassword(string Password, string ConfirmPassword)
        {
            if (string.IsNullOrWhiteSpace(Password)) throw new BadRequestException("Password Cannot be empty");
            if (Password != ConfirmPassword) throw new BadRequestException("New password confirmation must match the password");
            if (Password.Length < 8) throw new BadRequestException("New password must be at least 8 characters");
            if (!IsStrongPassword(Password)) throw new BadRequestException("Password is too weak");
        }

        private string NormalizeEmail(string email)
        {
            return email?.Trim().ToLowerInvariant() ?? string.Empty;
        }

        private async Task OldPasswordReuseCheckAndCycle(User user, string newPassword, CancellationToken cancellationToken)
        {
            if (hasher.Verify(user.PasswordHash, newPassword))
                throw new ConflictException("New password must be different from the current password.");

            const int MaxPasswordHistory = 3;
            var usedBeforePasswordsHash = await uow.userPasswordHistories.GetAllByUserIdAsync(user.Id, MaxPasswordHistory, cancellationToken);

            foreach (var pw in usedBeforePasswordsHash)
            {
                if (hasher.Verify(pw.PasswordHash, newPassword))
                    throw new ConflictException("You cannot reuse one of your recent passwords");
            }

            if (usedBeforePasswordsHash.Count >= MaxPasswordHistory)
            {
                uow.userPasswordHistories.Delete(usedBeforePasswordsHash[MaxPasswordHistory - 1]);
            }

            var currentPasswordSaveHistory = new UserPasswordHistory() { UserId = user.Id, PasswordHash = user.PasswordHash };

            await uow.userPasswordHistories.AddAsync(currentPasswordSaveHistory, cancellationToken);
        }
        #endregion ========================================================================

        public async Task<GeneratePasswordResetDto> RequestPasswordResetAsync(ForgotPasswordDto dto, CancellationToken cancellationToken)
        {
            var token = OTTService.GenerateToken();
            var hashedToken = OTTService.HashToken(token);
            var normalizedEmail = NormalizeEmail(dto.Email);

            var user = await uow.users.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (user == null)
                return new GeneratePasswordResetDto();

            var passwordResetToken = new PasswordResetToken() { UserId = user.Id, TokenHash = hashedToken };

            await uow.passwordResetTokens.InvalidateAllByUserIdAsync(user.Id, cancellationToken);
            await uow.passwordResetTokens.AddAsync(passwordResetToken, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            return new GeneratePasswordResetDto() { Email = user.Email, Token = token };
        }

        public async Task ResetPasswordAsync(string token, ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            var hashedToken = OTTService.HashToken(token);
            var newPassword = dto.NewPassword;
            var confirmPassword = dto.ConfirmPassword;

            ValidatePassword(newPassword, confirmPassword);

            var passwordResetToken = await uow.passwordResetTokens.GetByTokenHashAsync(hashedToken, cancellationToken);
            if (passwordResetToken == null || passwordResetToken.IsActive == false)
                throw new InvalidTokenException("Invalid or expired verification token.");

            var user = await uow.users.GetByIdAsync(passwordResetToken.UserId, cancellationToken);
            if (user == null)
                throw new NotFoundException("the user that you are trying to change password for is not found");

            await OldPasswordReuseCheckAndCycle(user, newPassword, cancellationToken);

            var newPasswordHash = hasher.Hash(newPassword);
            passwordResetToken.MarkAsUsed();
            user.ChangePassword(newPasswordHash);

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
