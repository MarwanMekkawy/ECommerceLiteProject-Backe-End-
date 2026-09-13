using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.ClientsAbstractions;
using IdentityService.Application.MappingProfiles;
using IdentityService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Application.Extentions.App
{
    public static class ApplicationServicesCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services) 
        {
            // DI
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailVerificationTokenService, EmailVerificationTokenService>();
            services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IServiceClientService, ServiceClientService>();
            services.AddScoped<ISelfServiceClientService, SelfServiceClientService>();
            services.AddScoped<ITokenCleanupService, TokenCleanupService>();

            services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(AutoMapperMarker).Assembly); });


            return services;
        }
    }
}
