namespace IdentityService.Application.Abstractions.ClientsAbstractions
{
    public interface INotificationServiceClient
    {
        Task SendEmailConfirmationAsync(Guid userId, string recipientEmail, string firstName, string confirmationUrl);
        Task SendPasswordResetAsync(Guid userId, string recipientEmail, string firstName, string resetUrl, int expirationMinutes);
        Task SendEmailChangedAsync(Guid userId, string recipientEmail, string firstName, string newEmail);
        Task SendPasswordChangedAsync(Guid userId, string recipientEmail, string firstName);
    }
}
