namespace IdentityService.Application.UseCases.Email.ResendVerification
{
    public interface IResendEamilVerificationUseCase
    {
        Task ResendAsync(Guid userId, CancellationToken cancellationToken);
    }
}
