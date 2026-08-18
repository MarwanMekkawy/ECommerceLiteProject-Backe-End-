using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OrderService.API.BackgroundTasks;
using OrderService.API.Middleware;
using OrderService.Application.Extentions.App;
using OrderService.InfraStructure.Extentions.Infra;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;



namespace OrderService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Di Services extentions
            builder.Services.AddInfrastructureServices(builder.Configuration).AddApplicationServices();

            // Register the background cleaning expired pending orders
            builder.Services.AddHostedService<CancelExpiredOrdersBackgroundService>();

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
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
                    };
                    // Human bearer Authentication error msgs
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                            return context.Response.WriteAsJsonAsync(new { error = "Authentication required." });
                        },

                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;

                            return context.Response.WriteAsJsonAsync(new { error = "You are not authorized to perform this action." });
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
