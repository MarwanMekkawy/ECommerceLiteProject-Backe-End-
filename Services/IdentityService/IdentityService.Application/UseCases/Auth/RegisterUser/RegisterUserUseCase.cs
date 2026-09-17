using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.ClientsAbstractions;
using IdentityService.Application.DTOs.AuthDTOs;

namespace IdentityService.Application.UseCases.Auth.RegisterUser
{
    public class RegisterUserUseCase(IAuthService authService, IEmailVerificationTokenService emailVerification, INotificationServiceClient notificationsClient) : IRegisterUserUseCase
    {
        public async Task RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken)
        {
            var registerResultUserId = await authService.RegisterAsync(dto, cancellationToken);

            var emailVerificationTokenResult = await emailVerification.GenerateVerificationTokenAsync(registerResultUserId.userId, cancellationToken);

            await notificationsClient.SendEmailConfirmationAsync(registerResultUserId.userId, dto.Email, dto.FirstName, emailVerificationTokenResult.Token, cancellationToken);

            return ;
        }
    }
}