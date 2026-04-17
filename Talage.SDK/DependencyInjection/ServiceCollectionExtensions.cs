using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Talage.SDK.Configuration;
using Talage.SDK.Interfaces;
using Talage.SDK.Logging;
using Talage.SDK.Services;
using Talage.SDK.Internal.Auth;
using Talage.SDK.EntityFramework.Repository;
using Talage.SDK.EntityFramework.TalageIntegration.Context;
using TalageIntegration.Shared.Exceptions;
using TalageApiHttpClient = Talage.SDK.Internal.ApiClient.TalageApiClient;
using TalageAuthenticationHandler = Talage.SDK.Internal.ApiClient.TalageAuthenticationDelegatingHandler;
using QuotePollingGuard = Talage.SDK.Internal.ApiClient.QuotePollingGuard;
using ApplicationCreationLogService = Talage.SDK.Internal.Services.ApplicationCreationLogService;
using NullApplicationCreationLogService = Talage.SDK.Internal.Services.NullApplicationCreationLogService;

namespace Talage.SDK.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTalageSdk(this IServiceCollection services, Action<TalageSettings> configure)
    {
        var configuration = ResolveConfiguration(services);
        return services.AddTalageSdk(configuration, configure);
    }

    public static IServiceCollection AddTalageSdk(this IServiceCollection services, IConfiguration configuration, Action<TalageSettings> configure)
    {
        configuration ??= new ConfigurationBuilder().Build();

        services.Configure(configure);
        services.AddSingleton<IConfigureOptions<TalageApiOptions>, TalageApiOptionsConfigurator>();
        services.Configure<TalageApiOptions>(_ => { });

        var connectionString = configuration.GetConnectionString("TalageIntegration");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<TalageIntegrationContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<ITalageIntegrationRepository, TalageIntegrationRepository>();
            services.AddScoped<IAccessTokenStore, AccessTokenStore>();
            services.AddScoped<Talage.SDK.Internal.Interfaces.IApplicationCreationLogService, ApplicationCreationLogService>();
        }
        else
        {
            services.AddSingleton<IAccessTokenStore, NullAccessTokenStore>();
            services.AddSingleton<Talage.SDK.Internal.Interfaces.IApplicationCreationLogService, NullApplicationCreationLogService>();
        }

        services.AddTransient<TalageAuthenticationHandler>();
        services.AddScoped<ITalageTokenManager, TalageTokenManager>();
        services.AddScoped<ITalageTokenProvider, TalageTokenProvider>();
        services.AddSingleton<Talage.SDK.Internal.Interfaces.IQuotePollingGuard, QuotePollingGuard>();

        services.AddHttpClient<Talage.SDK.Internal.Interfaces.ITelangeService, TelangeService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TalageApiOptions>>().Value;
            client.BaseAddress = BuildTalageApiBaseAddress(options.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddHttpClient<Talage.SDK.Internal.Interfaces.ITalageApiClient, TalageApiHttpClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TalageApiOptions>>().Value;
            client.BaseAddress = BuildTalageApiBaseAddress(options.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddHttpMessageHandler<TalageAuthenticationHandler>();

        services.AddScoped<IApiLogger, ApiLogger>();
        services.AddScoped<ITalageClient, TalageClient>();

        return services;
    }

    private static IConfiguration ResolveConfiguration(IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(x => x.ServiceType == typeof(IConfiguration));
        return descriptor?.ImplementationInstance as IConfiguration ?? new ConfigurationBuilder().Build();
    }

    private static Uri BuildTalageApiBaseAddress(string baseUrl)
    {
        var trimmed = (baseUrl ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new TalageConfigurationException("Talage BaseUrl is required.");
        }

        trimmed = trimmed.TrimEnd('/');
        return new Uri($"{trimmed}/v1/api/");
    }

    private sealed class TalageApiOptionsConfigurator(IOptions<TalageSettings> settings) : IConfigureOptions<TalageApiOptions>
    {
        public void Configure(TalageApiOptions options)
        {
            options.BaseUrl = settings.Value.BaseUrl;
            options.ApiKey = settings.Value.ApiKey;
            options.ApiSecret = settings.Value.Secret;
        }
    }
}

