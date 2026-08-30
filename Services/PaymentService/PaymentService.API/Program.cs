
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PaymentService.API.Middlewere;
using PaymentService.Application.Extentions.App;
using PaymentService.Infrastructure.Extentions.Infra;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace PaymentService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Di Services extentions
            builder.Services.AddInfrastructureServices(builder.Configuration).AddApplicationServices();

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

            builder.Services.AddAuthorization();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Development", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });

                options.AddPolicy("Production", policy =>
                {
                    policy.WithOrigins("https://myfrontend.com")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            app.UseHttpsRedirection();

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
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
