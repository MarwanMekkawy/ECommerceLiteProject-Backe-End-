using NotificationService.Application.Abstractions;

namespace NotificationService.API.BackgroundTasks
{
    public class PendingNotificationBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateScope();

                var sendPendingService = scope.ServiceProvider.GetRequiredService<INotificationDeliveryService>();

                await sendPendingService.ProcessPendingNotificationsAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }    
}
