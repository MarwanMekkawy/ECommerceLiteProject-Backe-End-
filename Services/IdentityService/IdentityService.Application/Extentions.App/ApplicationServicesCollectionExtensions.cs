using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.ClientsAbstractions;
using IdentityService.Application.MappingProfiles;
using IdentityService.Application.Services;
using IdentityService.Application.UseCases.Auth.RegisterUser;
using IdentityService.Application.UseCases.Email.ChangeEmail;
using IdentityService.Application.UseCases.Email.ConfirmEmailChange;
using IdentityService.Application.UseCases.Email.ResendVerification;
using IdentityService.Application.UseCases.Password.ForgotPassword;
using IdentityService.Application.UseCases.Password.ResetPassword;
using IdentityService.Application.UseCases.User.ChangePassword;
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

            //usecases
            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            services.AddScoped<IChangeEmailVerificationUseCase, ChangeEmailVerificationUseCase>();
            services.AddScoped<IConfirmEmailChangeUseCase, ConfirmEmailChangeUseCase>();
            services.AddScoped<IResendEamilVerificationUseCase, ResendEamilVerificationUseCase>();
            services.AddScoped<IForgotPasswordVerificationUseCase, ForgotPasswordVerificationUseCase>();
            services.AddScoped<IResetPasswordConfirmaionUseCase, ResetPasswordConfirmaionUseCase>();
            services.AddScoped<IPasswordChangedConfirmationUseCase, PasswordChangedConfirmationUseCase>();


            services.AddAutoMapper(cfg => { cfg.AddMaps(typeof(AutoMapperMarker).Assembly); });


            return services;
        }
    }
}
