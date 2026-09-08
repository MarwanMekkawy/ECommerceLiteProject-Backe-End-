using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Infrastructure.Extentions.Infra
{
    public static class DbContextConfiguration
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<NotificationDbContext>(options =>
            { options.UseSqlServer(config.GetConnectionString("NotificationSqlServerConnection")); }
            );

            return services;
        }
    }
}
