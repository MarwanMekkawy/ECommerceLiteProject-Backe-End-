using IdentityService.Application.Abstractions;
using IdentityService.Application.MappingProfiles;
using IdentityService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(AutoMapperMarker).Assembly); });

            services.AddScoped<ITokenCleanupService, TokenCleanupService>();

            return services;
        }
    }
}
