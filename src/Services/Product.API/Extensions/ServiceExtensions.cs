﻿using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories;
using Product.API.Repositories.Interfaces;

namespace Product.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureProductDbContext(configuration);
        services.AddInfrastructureServices();
        //services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
        // services.AddJwtAuthentication();
        
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
}