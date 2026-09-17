using IdentityService.Application.DTOs.PwResetDTOs;

namespace IdentityService.Application.UseCases.Password.ForgotPassword
{
    public interface IForgotPasswordVerificationUseCase
    {
        Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken);
    }
}
