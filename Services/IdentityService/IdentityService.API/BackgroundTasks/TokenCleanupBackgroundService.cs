using IdentityService.Application.Abstractions;

namespace IdentityService.API.BackgroundTasks
{
    public class TokenCleanupBackgroundService( IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateScope();

                var cleanupService = scope.ServiceProvider.GetRequiredService<ITokenCleanupService>();

                await cleanupService.CleanupAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromHours(48), stoppingToken);
            }
        }
    }
}
