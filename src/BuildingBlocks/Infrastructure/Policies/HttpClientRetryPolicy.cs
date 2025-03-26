using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Infrastructure.Policies;

public static class HttpClientRetryPolicy
{
    public static IHttpClientBuilder UseImmediateHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ImmediateHttpRetry());
    }
    
    public static IHttpClientBuilder UseLinearHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(LinearHttpRetry());
    }
    
    public static IHttpClientBuilder UseExponentialHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ExponentialHttpRetry());
    }
    
    public static IHttpClientBuilder UseCircuitHttpRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(ConfigureCircuitBreakerPolicy());
    }
    
    public static IHttpClientBuilder ConfigureTimeoutPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(5));
    }

    private static IAsyncPolicy<HttpResponseMessage> ImmediateHttpRetry() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .RetryAsync(3, (ex, retryCount, ctx) =>
            {
                Log.Error($"Retry {retryCount} of {ctx.PolicyKey} at {ctx.OperationKey}");
            });
    
    private static IAsyncPolicy<HttpResponseMessage> LinearHttpRetry() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(3), 
                (ex, retryCount, ctx) =>
            {
                Log.Error($"Retry {retryCount} of {ctx.PolicyKey} at {ctx.OperationKey}");
            });
    
    private static IAsyncPolicy<HttpResponseMessage> ExponentialHttpRetry() =>
        // 2^1 = 2s
        // 2^2 = 4s
        // 2^3 = 8s
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(Math.Pow(2, x)), 
                (ex, retryCount, ctx) =>
                {
                    Log.Error($"Retry {retryCount} of {ctx.PolicyKey} at {ctx.OperationKey}");
                });
    
    private static IAsyncPolicy<HttpResponseMessage> ConfigureCircuitBreakerPolicy() =>
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
}