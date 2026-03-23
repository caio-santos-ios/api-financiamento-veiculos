using System.Text;
using api_financiamento.src.Interfaces;
using api_financiamento.src.Repository;
using api_financiamento.src.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace api_financiamento.src.Configuration
{
    public static class Build
    {
        public static void AddBuilderConfiguration(this WebApplicationBuilder builder)
        {
            AppDbContext.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "";
            AppDbContext.DatabaseName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "";
            bool IsSSL;
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IS_SSL")))
            {
                IsSSL = Convert.ToBoolean(Environment.GetEnvironmentVariable("IS_SSL"));
            }
            else
            {
                IsSSL = false;
            }

            AppDbContext.IsSSL = IsSSL;
        }

        public static void AddBuilderAuthentication(this WebApplicationBuilder builder)
        {
            string? SecretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? "";
            string? Issuer = Environment.GetEnvironmentVariable("ISSUER") ?? "";
            string? Audience = Environment.GetEnvironmentVariable("AUDIENCE") ?? "";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(SecretKey)
                    )
                };
            });
        }

        public static void AddContext(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<AppDbContext>();
        }

        public static void AddBuilderServices(this WebApplicationBuilder builder)
        {
            // SIMULATOR
            builder.Services.AddTransient<ISimulationService, SimulationService>();
            builder.Services.AddTransient<ISimulationRepository, SimulationRepository>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
