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
        public static async Task SeedAsync(IdentityDbContext context, IConfiguration configuration, IPasswordHasher passwordHasher, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("DatabaseSeeder");

            if (await context.Users.AnyAsync(u => u.Role == RoleType.Admin))
            {
                logger.LogInformation("Default admin already exists. Skipping admin seeding.");
                return;
            }

            var adminConfig = configuration.GetSection("DefaultAdmin");

            var password = adminConfig["Password"] ?? throw new InvalidOperationException("Default admin password is not configured.");

            var admin = new User
            {
                FirstName = adminConfig["FirstName"] ?? "System",
                LastName = adminConfig["LastName"] ?? "Admin",
                Email = adminConfig["Email"] ?? throw new InvalidOperationException("Default admin email is not configured."),
                PasswordHash = passwordHasher.Hash(password),
                IsEmailConfirmed = true,
                IsActive = true,
                Role = RoleType.Admin
            };

            context.Users.Add(admin);

            await context.SaveChangesAsync();

            logger.LogInformation("Default admin seeded successfully. Email: {Email}", admin.Email);
        }
    }
}
