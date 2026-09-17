using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.ClientsAbstractions;
using IdentityService.Application.DTOs.PwResetDTOs;

namespace IdentityService.Application.UseCases.Password.ResetPassword
{
    public class ResetPasswordConfirmaionUseCase(IPasswordResetTokenService passwordService, INotificationServiceClient notificationsClient) : IResetPasswordConfirmaionUseCase
    {
        public async Task SendPasswordResetConfirmationAsync(string token, ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            var PwResetResult = await passwordService.ResetPasswordAndLogOutAllDevicesAsync(token, dto, cancellationToken);

            await notificationsClient.SendPasswordChangedAsync(PwResetResult.userId, PwResetResult.Email, PwResetResult.firstName, cancellationToken);
        }
    }
}
