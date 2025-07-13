using IdentityServer4.AccessTokenValidation;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.Configurations;

namespace Infrastructure.Identity;

public static class ConfigAuthenticationHandler
{
    public static void ConfigAuthentication(this IServiceCollection serviceCollection)
    {
        var config = serviceCollection.GetOptions<ApiConfigSetting>("ApiConfig");

        var issuer = config.IssuerUri;
        var apiName = config.ApiName;
        
        serviceCollection.AddAuthentication("Bearer").AddIdentityServerAuthentication(op =>
        {
            op.ApiName = apiName;
            op.Authority = issuer;
            op.RequireHttpsMetadata = false;
            op.SupportedTokens = SupportedTokens.Both;
        });
    }

    public static void ConfigAuthorization(this IServiceCollection service)
    {
        service.AddAuthorization(op =>
        {
            op.AddPolicy("Bearer", po =>
            {
                po.AddAuthenticationSchemes("Bearer");
                po.RequireAuthenticatedUser();
            });
        });
    }
}