using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure.Extentions.Infra
{
    public static class DbContextConfiguration
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<IdentityDbContext>(options =>
            {options.UseSqlServer(config.GetConnectionString("IdentitySqlServerConnection"));}
            );

            return services;
        }
    }
}

