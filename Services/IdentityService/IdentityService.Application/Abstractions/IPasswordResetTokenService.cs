using IdentityService.Application.DTOs.PwResetDTOs;

namespace IdentityService.Application.Abstractions
{
    public interface IPasswordResetTokenService
    {
        Task<GeneratePasswordResetDto?> RequestPasswordResetAsync(ForgotPasswordDto dto, CancellationToken cancellationToken);
        Task<(Guid userId, string firstName, string Email)> ResetPasswordAndLogOutAllDevicesAsync(string token, ResetPasswordDto dto, CancellationToken cancellationToken);
    }
}
