namespace IdentityService.Application.Abstractions
{
    public interface ITokenCleanupService
    {
        Task CleanupAsync(CancellationToken cancellationToken);
    }
}
