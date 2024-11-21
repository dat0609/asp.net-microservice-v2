﻿using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        var con = configuration.GetConnectionString("DefaultConnectionString");
        if (con == null || string.IsNullOrEmpty(con))
            throw new ArgumentNullException("Connection string is not configured.");
        
        services.AddDbContext<OrderContext>(options =>
        {
            options.UseSqlServer(con,
                builder => 
                    builder.MigrationsAssembly(typeof(OrderContext).Assembly.FullName));
        });

        services.AddScoped<OrderContextSeed>();
        //services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

        //services.AddScoped(typeof(ISmtpEmailService), typeof(SmtpEmailService));

        return services;
    }
}