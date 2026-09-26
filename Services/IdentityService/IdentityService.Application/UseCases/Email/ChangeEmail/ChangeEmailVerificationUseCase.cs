using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.ClientsAbstractions;
using IdentityService.Application.DTOs.EmailVerificationDTOs;

namespace IdentityService.Application.UseCases.Email.ChangeEmail
{
    public class ChangeEmailVerificationUseCase(IEmailVerificationTokenService emailVerificationService, INotificationServiceClient notificationsClient) : IChangeEmailVerificationUseCase
    {
        public async Task EmailChangeAsync(Guid userId, ChangeEmailRequestDto dto, CancellationToken cancellationToken)
        {
            var tokenResult = await emailVerificationService.GenerateEmailChangeTokenAsync(userId, dto, cancellationToken);

            var expiresAt = DateTime.UtcNow.AddMinutes(1440).ToString("dd MMMM yyyy, HH:mm 'UTC'");
            await notificationsClient.SendEmailChangeConfirmationAsync(userId, dto.NewEmail, tokenResult.firstName, dto.NewEmail, tokenResult.token, expiresAt, cancellationToken);
        }
    }
}
