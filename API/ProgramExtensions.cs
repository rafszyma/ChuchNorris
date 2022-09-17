using System.Text;
using API.Interfaces;
using API.Services;
using Business.Interfaces;
using Business.Persistence;
using Business.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace API;

public static class ProgramExtensions
{
    public static void RegisterDi(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.Configure<TokenSettings>(builder.Configuration.GetSection("Token"));
        services.Configure<DbSettings>(builder.Configuration.GetSection("MongoDb"));
        services.AddScoped<UserService>();
        services.AddScoped<IQuoteService, QuoteService>();
        services.AddSingleton<IMongoDbContext, ChuckDbContext>();
    }

    public static void ConfigureLogger(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
    }

    public static void AddChuckAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var tokenSection = builder.Configuration.GetSection("Token");
                options.IncludeErrorDetails = true;
                options.Audience = tokenSection.GetSection("Audience").Value;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = tokenSection.GetSection("Issuer").Value,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSection.GetSection("Secret").Value)),
                };
            });
    }
    
    public static void ConfigureSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}