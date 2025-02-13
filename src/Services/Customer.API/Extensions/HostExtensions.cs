﻿using Common.Logging;
using Serilog;

namespace Customer.API.Extensions;

public static class HostExtensions
{
    internal static void AddAppConfigurations(this ConfigureHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            var env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, 
                    reloadOnChange: true)
                .AddEnvironmentVariables();
        }).UseSerilog(Serilogger.Configure);
    }

    /*internal static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, IConfiguration configuration)
    {
        var configDashboard = configuration.GetSection("HangFireSettings:Dashboard").Get<DashboardOptions>();
        var hangFireSettings = configuration.GetSection("HangFireSettings").Get<HangfireSettings>();
        var hangFireRoute = hangFireSettings.Route;

        app.UseHangfireDashboard(hangFireRoute, new DashboardOptions
        {
            // Authorization = new[] {new HangfireAuthorizationFilter()},
            DashboardTitle = configDashboard.DashboardTitle,
            StatsPollingInterval = configDashboard.StatsPollingInterval,
            AppPath = configDashboard.AppPath,
            IgnoreAntiforgeryToken = true
        });

        return app;
    }*/
}