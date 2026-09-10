
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NotificationService.API.BackgroundTasks;
using NotificationService.API.Middlewere;
using NotificationService.Application.Extentions.App;
using NotificationService.Infrastructure.Extentions.Infra;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace NotificationService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Di Services extentions
            builder.Services.AddInfrastructureServices(builder.Configuration).AddApplicationServices();

            // Register the {background} retry to complete paid orders or refund them
            builder.Services.AddHostedService<PendingNotificationBackgroundService>();
            builder.Services.AddHostedService<RetryFailedMailsBackgroundService>();

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                // swagger XML comments config //
                options => {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                    options.IncludeXmlComments(xmlPath);
                });

            //====== Auth JWT config ======//
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "ServiceJwt";
                options.DefaultChallengeScheme = "ServiceJwt";
            }).AddJwtBearer("ServiceJwt", options =>
            {
                var rsa = RSA.Create();
                rsa.ImportFromPem(builder.Configuration["JwtForServiceClient:PublicKey"]!.Replace("\\n", "\n"));
                var key = new RsaSecurityKey(rsa);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = key
                };

                // Service bearer Authentication error msgs
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Invalid or expired authentication token."
                        });
                    }
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("Development");                     // allow all CORS
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseCors("Production");
            }

            app.UseMiddleware<GlobalHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
