using System.Diagnostics.CodeAnalysis;
using EPR.Common.Authorization.Extensions;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using StackExchange.Redis;
using CookieOptions = Epr.Reprocessor.Exporter.UI.App.Options.CookieOptions;
using SessionOptions = Epr.Reprocessor.Exporter.UI.App.Options.SessionOptions;

namespace Epr.Reprocessor.Exporter.UI.Extensions;

//using Microsoft.Extensions.Time.Testing;

[ExcludeFromCodeCoverage]
public static class ServiceProviderExtension
{
    public static IServiceCollection RegisterWebComponents(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureOptions(services, configuration);
        //ConfigureLocalization(services);
        // TODO - Reinstate for B2C authentication
        //ConfigureAuthentication(services, configuration);
        //ConfigureAuthorization(services, configuration);
        ConfigureSession(services);
        // RegisterServices(services);
        // RegisterHttpClients(services, configuration);

        return services;
    }

    public static IServiceCollection ConfigureMsalDistributedTokenOptions(this IServiceCollection services)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddApplicationInsights());
        var buildLogger = loggerFactory.CreateLogger<Program>();

        services.Configure<MsalDistributedTokenCacheAdapterOptions>(options =>
        {
            var msalOptions = services.BuildServiceProvider().GetRequiredService<IOptions<MsalOptions>>().Value;

            options.DisableL1Cache = msalOptions.DisableL1Cache;
            options.SlidingExpiration = TimeSpan.FromMinutes(msalOptions.L2SlidingExpiration);

            options.OnL2CacheFailure = exception =>
            {
                if (exception is RedisConnectionException)
                {
                    buildLogger.LogError(exception, "L2 Cache Failure Redis connection exception: {Message}", exception.Message);
                    return true;
                }

                buildLogger.LogError(exception, "L2 Cache Failure: {Message}", exception.Message);
                return false;
            };
        });

        return services;
    }

    private static void ConfigureAuthorization(IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;

            var azureB2COptions = services.BuildServiceProvider().GetRequiredService<IOptions<AzureAdB2COptions>>().Value;

            options.LoginPath = azureB2COptions.SignedOutCallbackPath;
            options.AccessDeniedPath = azureB2COptions.SignedOutCallbackPath;

            options.SlidingExpiration = true;
        });

        services.RegisterPolicy<ReprocessorExporterRegistrationSession>(configuration);
    }

    private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GlobalVariables>(configuration);
        services.Configure<CookieOptions>(configuration.GetSection(CookieOptions.ConfigSection));
        services.Configure<MsalOptions>(configuration.GetSection(MsalOptions.ConfigSection));
        services.Configure<SessionOptions>(configuration.GetSection(SessionOptions.ConfigSection));
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.ConfigSection));
    }

    private static void RegisterServices(IServiceCollection services)
    {
    }


    private static void RegisterHttpClients(IServiceCollection services, IConfiguration configuration)
    {
        //TODO: Register required HTTP Clients
        //services.AddHttpClient<IAccountServiceApiClient, AccountServiceApiClient>((sp, client) =>
        //{
        //    var facadeApiOptions = sp.GetRequiredService<IOptions<AccountsFacadeApiOptions>>().Value;
        //    var httpClientOptions = sp.GetRequiredService<IOptions<HttpClientOptions>>().Value;

        //    client.BaseAddress = new Uri(facadeApiOptions.BaseEndpoint);
        //    client.Timeout = TimeSpan.FromSeconds(httpClientOptions.TimeoutSeconds);
        //});
    }

    private static void ConfigureLocalization(IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources")
            .Configure<RequestLocalizationOptions>(options =>
            {
                var cultureList = new[] { Language.English, Language.Welsh };
                options.SetDefaultCulture(Language.English);
                options.AddSupportedCultures(cultureList);
                options.AddSupportedUICultures(cultureList);
                options.RequestCultureProviders = new IRequestCultureProvider[]
                {
                    new SessionRequestCultureProvider(),
                };
            });
    }

    private static void ConfigureSession(IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();
        var globalVariables = sp.GetRequiredService<IOptions<GlobalVariables>>().Value;

        if (!globalVariables.UseLocalSession)
        {
            var redisOptions = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            var redisConnectionString = redisOptions.ConnectionString;

            //TODO: Check if Required
            //services.AddDataProtection()
            //    .SetApplicationName("EprProducers")
            //    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisConnectionString), "DataProtection-Keys");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = redisOptions.InstanceName;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddSession(options =>
        {
            var cookieOptions = sp.GetRequiredService<IOptions<CookieOptions>>().Value;
            var sessionOptions = sp.GetRequiredService<IOptions<SessionOptions>>().Value;

            options.Cookie.Name = cookieOptions.SessionCookieName;
            options.IdleTimeout = TimeSpan.FromMinutes(sessionOptions.IdleTimeoutMinutes);
            options.Cookie.IsEssential = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.Path = "/";
        });
    }

    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var sp = services.BuildServiceProvider();
        var cookieOptions = sp.GetRequiredService<IOptions<CookieOptions>>().Value;
        var facadeApiOptions = sp.GetRequiredService<IOptions<AccountsFacadeApiOptions>>().Value;

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(
                options =>
                {
                    configuration.GetSection(AzureAdB2COptions.ConfigSection).Bind(options);

                    options.CorrelationCookie.Name = cookieOptions.CorrelationCookieName;
                    options.NonceCookie.Name = cookieOptions.OpenIdCookieName;
                    options.ErrorPath = "/error";
                    options.ClaimActions.Add(new CorrelationClaimAction());
                },
                options =>
                {
                    options.Cookie.Name = cookieOptions.AuthenticationCookieName;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieOptions.AuthenticationExpiryInMinutes);
                    options.SlidingExpiration = true;
                    options.Cookie.Path = "/";
                })
            .EnableTokenAcquisitionToCallDownstreamApi(new[] { facadeApiOptions.DownstreamScope })
            .AddDistributedTokenCaches();
    }
}