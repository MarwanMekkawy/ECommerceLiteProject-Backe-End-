using OrderService.Application.Abstractions;

namespace OrderService.API.BackgroundTasks
{
    public class CancelExpiredOrdersBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) 
            {
                using var scope = scopeFactory.CreateScope();

                var cancelExpiredService = scope.ServiceProvider.GetRequiredService<ICancelExpiredOrdersService>();

                await cancelExpiredService.CancelExpiredAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
