namespace IdentityService.Application.Abstractions.ClientsAbstractions
{
    public interface INotificationServiceClient
    {
        Task SendEmailConfirmationAsync(Guid userId, string recipientEmail, string firstName, string emailConfirmationToken, CancellationToken cancellationToken);
        Task SendEmailChangedAsync(Guid userId, string recipientEmail, string firstName, string newEmail, CancellationToken cancellationToken);
        Task SendEmailChangeConfirmationAsync(Guid userId, string recipientEmail, string firstName, string newEmail, string emailChangeToken, int expirationMinutes, CancellationToken cancellationToken);
        Task SendPasswordResetAsync(Guid userId, string recipientEmail, string firstName, string passwordResetToken, int expirationMinutes, CancellationToken cancellationToken);
        Task SendPasswordChangedAsync(Guid userId, string recipientEmail, string firstName, CancellationToken cancellationToken);
    }
}
