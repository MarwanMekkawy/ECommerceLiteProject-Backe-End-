
using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.ClientsAbstractions;

namespace IdentityService.Application.UseCases.Email.ResendVerification
{
    public class ResendEamilVerificationUseCase(IEmailVerificationTokenService emailVerificationService, INotificationServiceClient notificationsClient) : IResendEamilVerificationUseCase
    {
        public async Task ResendAsync(Guid userId, CancellationToken cancellationToken)
        {
            var result = await emailVerificationService.ResendVerificationEmailAsync(userId, cancellationToken);

            await notificationsClient.SendEmailConfirmationAsync(userId, result.dto.Email, result.firstName, result.dto.Token, cancellationToken);
        }
    }
}
