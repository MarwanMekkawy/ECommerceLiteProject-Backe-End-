using IdentityService.Application.DTOs.UserDTOs;

namespace IdentityService.Application.UseCases.User.ChangePassword
{
    public interface IPasswordChangedConfirmationUseCase
    {
        Task SendPasswordResetConfirmationAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken);
    }
}
