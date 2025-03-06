using Saga.Orc.HttpRepositories;
using Saga.Orc.HttpRepositories.Interfaces;
using Saga.Orc.Services;
using Saga.Orc.Services.Interfaces;

namespace Saga.Orc.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<ICheckoutSagaService, CheckoutSagaService>();
    }

    public static void ConfigureHttpRepository(this IServiceCollection services)
    {
        services.AddScoped<IOrderHttpRepository, OrderHttpRepository>();
        services.AddScoped<IBasketHttpRepository, BasketHttpRepository>();
        services.AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();
    }

    public static void ConfigureHttpClients(this IServiceCollection services)
    {
        ConfigureInventoryHttp(services);
        ConfigureOrderHttp(services);
        ConfigureBasketHttp(services);
    }

    private static void ConfigureOrderHttp(this IServiceCollection services)
    {
        services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrdersAPI", (provider, client) =>
        {
            client.BaseAddress = new Uri("http://localhost:5006/api/v1/");
        });
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("OrdersAPI"));
    }
    
    private static void ConfigureBasketHttp(this IServiceCollection services)
    {
        services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketsAPI", (provider, client) =>
        {
            client.BaseAddress = new Uri("http://localhost:5004/api/");
        });
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("BasketsAPI"));
    }
    
    private static void ConfigureInventoryHttp(this IServiceCollection services)
    {
        services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventoryAPI", (provider, client) =>
        {
            client.BaseAddress = new Uri("http://localhost:5010/api/");
        });
        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("InventoryAPI"));
    }
}