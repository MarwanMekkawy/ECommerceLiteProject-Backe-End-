using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.ClientsAbstractions;
using IdentityService.Application.DTOs.UserDTOs;

namespace IdentityService.Application.UseCases.User.ChangePassword
{
    public class PasswordChangedConfirmationUseCase(IUserService userService, INotificationServiceClient notificationsClient) : IPasswordChangedConfirmationUseCase
    {
        public async Task SendPasswordResetConfirmationAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken)
        {
            var result = await userService.ChangePasswordAndLogOutAllDevicesAsync(userId, dto, cancellationToken);

            await notificationsClient.SendPasswordChangedAsync(userId, result.email, result.firstName, cancellationToken);
        }
    }
}
