using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace IdentityService.Infrastructure.Data.Seeding
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync (IdentityDbContext context, IConfiguration configuration, 
            IPasswordHasher passwordHasher, IOneTimeTokenService oTTokenService, ILoggerFactory loggerFactory)
        {
            var Logger = loggerFactory.CreateLogger("DatabaseSeeder");

            await SeedAdminAsync(context, configuration, passwordHasher, oTTokenService, Logger);

            await SeedServicesAsync(context, configuration, oTTokenService, Logger);

            await context.SaveChangesAsync();

            Logger.LogInformation("================> +++++ All Seeding were Done. +++++");
        }

        #region //seeding Default Admin ========================================================================================================
        private static async Task SeedAdminAsync(IdentityDbContext context, IConfiguration configuration,
            IPasswordHasher passwordHasher, IOneTimeTokenService oTTokenService, ILogger logger)
        {
            if (await context.Users.AnyAsync(u => u.Role == RoleType.Admin))
            {
                logger.LogInformation("================> Default admin already exists. Skipping admin seeding.");
                return;
            }

            var adminConfig = configuration.GetSection("DefaultAdmin");

            var password = adminConfig["Password"] ?? throw new InvalidOperationException("================> Default admin password is not configured.");

            var admin = new User
            {
                FirstName = adminConfig["FirstName"] ?? "System",
                LastName = adminConfig["LastName"] ?? "Admin",
                Email = adminConfig["Email"] ?? throw new InvalidOperationException("================> Default admin email is not configured."),
                PasswordHash = passwordHasher.Hash(password),
                IsEmailConfirmed = true,
                IsActive = true,
                Role = RoleType.Admin
            };

            context.Users.Add(admin);
            logger.LogInformation("================> +++++ admin seeded. +++++");
        }
        #endregion =============================================================================================================================

        #region //seeding Service Clients ======================================================================================================
        private static async Task SeedServicesAsync
            (IdentityDbContext context, IConfiguration configuration, IOneTimeTokenService oTTokenService, ILogger logger)
        {
            var services = configuration.GetSection("servicesSecrets");

            if(!services.Exists())
                logger.LogInformation("===============> No Configrations Were Found. <===============");

            foreach (var child in services.GetChildren())
            {
                var serviceName = child.Key;
                var serviceId = child["ServiceId"] ?? throw new InvalidOperationException($"================> {serviceName} service id is not configured.");
                var serviceSecret = child["ServiceSecret"] ?? throw new InvalidOperationException($"================> {serviceName} service secret is not configured.");

                if (await context.ServiceClients.AnyAsync(s => s.ServiceName == serviceName))
                {
                    logger.LogInformation($"================> {serviceName} already exists. Skipping {serviceName} seeding.");
                    continue;
                }
                var hashedSecret = oTTokenService.HashToken(serviceSecret);

                var service = new ServiceClient(serviceId, hashedSecret, serviceName);
                
                context.ServiceClients.Add(service);

                logger.LogInformation($"================> +++++ {serviceName} seeded. +++++");
            }             
        }
        #endregion =============================================================================================================================
    }
}
