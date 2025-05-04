using Core.Interfaces;
using Infrastructure.Plugins;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Infrastructure.Services.Infrastructure.Services;
using Infrastructure.Services.Plugins;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        IConfiguration configuration = context.Configuration;

        services.AddSingleton<IMonitoringService, MonitoringService>();
        services.AddSingleton<ISystemMetricsProvider, WindowsSystemMetricsProvider>();

        // Add plugins
        services.AddSingleton<IMonitorPlugin, ConsoleLoggerPlugin>();
        services.AddSingleton<IMonitorPlugin, FileLoggerPlugin>();

        // Register API Plugin
        services.AddHttpClient(); // Register factory

        services.AddSingleton<IMonitorPlugin>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            var endpoint = configuration["ApiSettings:Endpoint"];
            return new ApiMonitorPlugin(httpClient, endpoint);
        });

        services.AddSingleton<IMonitorPlugin>(
        new SlackAlertPlugin("https://hooks.slack.com/services/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"));

        


    })
    .Build();

var monitoringService = host.Services.GetRequiredService<IMonitoringService>();
await monitoringService.StartAsync();
