namespace IdentityService.Application.Abstractions.ClientsAbstractions
{
    public interface INotificationServiceClient
    {
        Task SendEmailConfirmationAsync(Guid userId, string recipientEmail, string firstName, string confirmationUrl, CancellationToken cancellationToken);
        Task SendPasswordResetAsync(Guid userId, string recipientEmail, string firstName, string resetUrl, int expirationMinutes, CancellationToken cancellationToken);
        Task SendEmailChangedAsync(Guid userId, string recipientEmail, string firstName, string newEmail, CancellationToken cancellationToken);
        Task SendPasswordChangedAsync(Guid userId, string recipientEmail, string firstName, CancellationToken cancellationToken);
    }
}
