using IdentityService.Application.DTOs.EmailVerificationDTOs;

namespace IdentityService.Application.UseCases.Email.ChangeEmail
{
    public interface IChangeEmailVerificationUseCase
    {
        Task EmailChangeAsync(Guid userId, ChangeEmailRequestDto dto, CancellationToken cancellationToken);
    }
}
