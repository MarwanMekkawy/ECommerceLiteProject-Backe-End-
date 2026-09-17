namespace IdentityService.Application.UseCases.Email.ConfirmEmailChange
{
    public interface IConfirmEmailChangeUseCase
    {
        Task SendEmailChangeConfirmationAsync(string token, CancellationToken cancellationToken);
    }
}
