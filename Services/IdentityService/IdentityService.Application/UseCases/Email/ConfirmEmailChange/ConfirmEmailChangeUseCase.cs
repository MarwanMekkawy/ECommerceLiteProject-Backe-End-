using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.ClientsAbstractions;

namespace IdentityService.Application.UseCases.Email.ConfirmEmailChange
{
    public class ConfirmEmailChangeUseCase(IEmailVerificationTokenService emailVerificationService, INotificationServiceClient notificationsClient) : IConfirmEmailChangeUseCase
    {
        public async Task SendEmailChangeConfirmationAsync(string token, CancellationToken cancellationToken)
        {
            var emailChangeResult = await emailVerificationService.ConfirmEmailChangeAsync(token, cancellationToken);

            await notificationsClient.SendEmailChangedAsync(emailChangeResult.UserId, emailChangeResult.OldEmail!, emailChangeResult.FirstName!, emailChangeResult.NewEmail!, cancellationToken);
        }
    }
}
