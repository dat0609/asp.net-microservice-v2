using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Infrastructure.Common;

namespace Basket.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection service) =>
        service.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializeService, SerializeService>();
    
    public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        //var settings = services.GetOptions<CacheSettings>(nameof(CacheSettings));
        var redisConnection = configuration.GetSection("CacheSettings:ConnectionString").Value;
        if (string.IsNullOrEmpty(redisConnection))
            throw new ArgumentNullException("Redis Connection string is not configured.");
        
        //Redis Configuration
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
        });
    }
}