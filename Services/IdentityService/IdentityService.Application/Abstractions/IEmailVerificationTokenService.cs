using IdentityService.Application.DTOs.EmailVerificationDTOs;

namespace IdentityService.Application.Abstractions
{
    public interface IEmailVerificationTokenService
    {       
        Task<GenerateVerificationEmailDto> GenerateVerificationTokenAsync(Guid userId, CancellationToken cancellationToken);
        Task<(GenerateVerificationEmailDto dto, string firstName)> ResendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken);
        Task ConfirmEmailAsync(string token, CancellationToken cancellationToken);

        Task<(string token, string firstName, string oldEmail)> GenerateEmailChangeTokenAsync(Guid userId, ChangeEmailRequestDto dto, CancellationToken cancellationToken);
        Task<ConfirmEmailChangeDto> ConfirmEmailChangeAsync(string token, CancellationToken cancellationToken);
    }
}
