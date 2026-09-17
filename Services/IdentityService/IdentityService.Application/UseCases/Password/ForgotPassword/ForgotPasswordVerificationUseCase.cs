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

            await notificationsClient.SendPasswordResetAsync
                (tokenResult.UserId, tokenResult.Email, tokenResult.FirstName, tokenResult.Token, tokenResult.ExpirationInMinutes, cancellationToken);
        }
    }
}
