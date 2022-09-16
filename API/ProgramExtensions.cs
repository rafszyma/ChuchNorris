using API.Interfaces;
using API.Persistence;
using API.Services;
using API.Settings;
using Buisness.Settings;
using Business.Interfaces;
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