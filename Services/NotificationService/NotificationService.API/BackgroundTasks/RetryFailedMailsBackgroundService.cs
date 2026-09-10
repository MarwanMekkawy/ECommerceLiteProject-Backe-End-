using NotificationService.Application.Abstractions;

namespace NotificationService.API.BackgroundTasks
{
    public class RetryFailedMailsBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateScope();

                var retryFailedService = scope.ServiceProvider.GetRequiredService<IRetrySendingFailedMailsService>();

                await retryFailedService.RetryFailedNotificationsAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }            
    }
}
