using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.InfraStructure.Extentions.Infra
{
    public static class DbContextConfiguration
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<OrderDbContext>(options =>
            { options.UseSqlServer(config.GetConnectionString("OrderSqlServerConnection")); }
            );

            return services;
        }
    }
}
