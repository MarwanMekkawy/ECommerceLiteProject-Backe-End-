using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.ClientsAbstractions;
using IdentityService.Application.DTOs.PwResetDTOs;

namespace IdentityService.Application.UseCases.Password.ForgotPassword
{
    public class ForgotPasswordVerificationUseCase(IPasswordResetTokenService passwordService, INotificationServiceClient notificationsClient) : IForgotPasswordVerificationUseCase
    {
        public async Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken)
        {
            var tokenResult = await passwordService.RequestPasswordResetAsync(dto, cancellationToken);

            if (tokenResult is null) return;

            var expiresAt = DateTime.UtcNow.AddMinutes(tokenResult.ExpirationInMinutes).ToString("dd MMMM yyyy, HH:mm 'UTC'");
            await notificationsClient.SendPasswordResetAsync(tokenResult.UserId, tokenResult.Email, tokenResult.FirstName, tokenResult.Token, expiresAt, cancellationToken);
        }
    }
}