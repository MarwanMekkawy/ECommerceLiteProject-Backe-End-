using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Application.Abstractions.ClientsAbstractions;
using IdentityService.Domain.Contracts;
using IdentityService.Infrastructure.Clients;
using IdentityService.Infrastructure.Clients.NotificationServiceClient;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Security;
using IdentityService.Infrastructure.SecurityRepos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

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


            services.AddHttpClient<INotificationServiceClient, NotificationServiceClient>(
                client => { client.BaseAddress = new Uri(config["HttpClients:NotificationService:BaseUrl"]!); });
            services.AddSingleton<IServiceTokenCache, ServiceTokenCache>();


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
