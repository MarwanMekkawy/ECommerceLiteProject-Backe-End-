using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentService.Infrastructure.Extentions.Infra
{
    public static class DbContextConfiguration
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<PaymentDbContext>(options =>
            { options.UseSqlServer(config.GetConnectionString("OrderSqlServerConnection")); }
            );

            return services;
        }
    }
}
