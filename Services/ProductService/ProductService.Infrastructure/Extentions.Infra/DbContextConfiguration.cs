using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ProductService.Infrastructure.Extentions.Infra
{
    public static class DbContextConfiguration
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ProductDbContext>(options =>
            { options.UseSqlServer(config.GetConnectionString("SqlServerConnection")); }
            );

            return services;
        }
    }
}
