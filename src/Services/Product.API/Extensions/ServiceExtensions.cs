using System.Text;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories;
using Product.API.Repositories.Interfaces;
using Shared.Configurations;

namespace Product.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.ConfigSwagger(configuration);
        services.ConfigureProductDbContext(configuration);
        services.AddInfrastructureServices();
        //services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
        // services.AddJwtAuthentication();
        services.ConfigAuthentication();
        services.ConfigAuthorization();
        services.ConfigureHealthChecks(configuration);
        return services;
    }
    
    private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        //var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        var databaseSettings = configuration.GetConnectionString("DefaultConnectionString");
        if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings))
            throw new ArgumentNullException("Connection string is not configured.");
        
        var builder = new MySqlConnectionStringBuilder(databaseSettings);
        services.AddDbContext<ProductContext>(m => m.UseMySql(builder.ConnectionString, 
            ServerVersion.AutoDetect(builder.ConnectionString), e =>
            {
                e.MigrationsAssembly("Product.API");
                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            }));

        return services;
    }
    
    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
                .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                .AddScoped<IProductRepository, ProductRepository>()
            ;
    }
    
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettings))
            .Get<JwtSettings>();
        services.AddSingleton(jwtSettings);

        var apiConfigSetting = configuration.GetSection("ApiConfig")
            .Get<ApiConfigSetting>();
        services.AddSingleton(apiConfigSetting);
        
        return services;
    }
    
    internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        var settings = services.GetOptions<JwtSettings>(nameof(JwtSettings));
        if (settings == null || string.IsNullOrEmpty(settings.Key))
            throw new ArgumentNullException($"{nameof(JwtSettings)} is not configured properly");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = false
        };
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.RequireHttpsMetadata = false;
            x.TokenValidationParameters = tokenValidationParameters;
        });

        return services;
    }

    private static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseSettings = configuration.GetConnectionString("DefaultConnectionString");
        services.AddHealthChecks()
            .AddMySql(databaseSettings);
    }

    public static void ConfigSwagger(this IServiceCollection service, IConfiguration configuration)
    {
        var apiConfigSetting = service.GetOptions<ApiConfigSetting>("ApiConfig");
        service.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Product",
                Version = "v1",
                Contact = new OpenApiContact()
                {
                    Email = "123@gmail.com",
                    Name = "Identity Service"
                }
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{apiConfigSetting.IdentityServerBaseUrl}/connect/authorize"),
                        Scopes = new Dictionary<string, string>
                        {
                            {"tedu_api_read", "Read Scope"},
                            {"tedu_api_write", "Write Scope"}
                        }
                    }
                }
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme,Id = "Bearer" }
                    },
                    new List<string>
                    {
                        "tedu_api_read",
                        "tedu_api_write"
                    }
                }
            });
        });
    }
}