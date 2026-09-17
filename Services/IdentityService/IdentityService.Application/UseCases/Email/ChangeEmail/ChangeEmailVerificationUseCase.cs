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

            await notificationsClient.SendEmailConfirmationAsync(userId, dto.NewEmail, tokenResult.firstName, tokenResult.token, cancellationToken);
        }
    }
}
