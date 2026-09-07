using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Security;
using IdentityService.Infrastructure.SecurityRepos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Extentions.Infra
{
    public static class InfrastructureServicesCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            //DbContext Connection String 
            services.AddAppDbContext(config);

            // DI registering
            services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
            services.AddScoped<IEmailChangeTokenRepository, EmailChangeTokenRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserPasswordHistoryRepository, UserPasswordHistoryRepository>();
            services.AddScoped<IServiceClientRepository, ServiceClientRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddSingleton<IOneTimeTokenService, OneTimeTokenService>();

            #region // RSA KEY Singletone register
            var privateKey = config["JwtForServiceClient:PrivateKey"];
            if (string.IsNullOrWhiteSpace(privateKey)) throw new InvalidOperationException("JWT service client private key is missing from configuration.");
            var rsa = RSA.Create();
            try
            {
                rsa.ImportFromPem(privateKey.Replace("\\n", "\n"));
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("JWT service client private key is invalid or has an invalid PEM format.", ex);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException("JWT service client private key could not be imported.", ex);
            }
            var rsaServiceSigningKey = new RsaSecurityKey(rsa);
            #endregion
            services.AddSingleton(rsaServiceSigningKey);

            return services;
        }
    }
}
