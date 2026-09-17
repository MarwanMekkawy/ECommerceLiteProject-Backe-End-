using IdentityService.Application.DTOs.PwResetDTOs;

namespace IdentityService.Application.UseCases.Password.ResetPassword
{
    public interface IResetPasswordConfirmaionUseCase
    {
        Task SendPasswordResetConfirmationAsync(string token, ResetPasswordDto dto, CancellationToken cancellationToken);
    }
}
