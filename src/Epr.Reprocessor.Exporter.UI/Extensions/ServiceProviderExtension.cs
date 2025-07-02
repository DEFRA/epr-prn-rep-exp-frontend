using Epr.Reprocessor.Exporter.UI.App.Clients.Interfaces;
using Epr.Reprocessor.Exporter.UI.App.Constants;
using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services;
using Epr.Reprocessor.Exporter.UI.Middleware;
using Epr.Reprocessor.Exporter.UI.Sessions;
using Epr.Reprocessor.Exporter.UI.ViewModels.Shared;
using EPR.Common.Authorization.Extensions;
using EPR.Common.Authorization.Models;
using EPR.Common.Authorization.Sessions;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.TokenCacheProviders.Distributed;
using StackExchange.Redis;
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Controllers;
using System.Security.Claims;
using Epr.Reprocessor.Exporter.UI.App.Helpers;
using CookieOptions = Epr.Reprocessor.Exporter.UI.App.Options.CookieOptions;
using Epr.Reprocessor.Exporter.UI.Mapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Epr.Reprocessor.Exporter.UI.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceProviderExtension
{
    public static IServiceCollection RegisterWebComponents(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
    {
        ConfigureOptions(services, configuration);
        ConfigureLocalization(services);
        ConfigureAuthentication(services, configuration);
        ConfigureAuthorization(services, configuration);
        ConfigureSession(services, configuration);
		RegisterServices(services, env);
        RegisterHttpClients(services, configuration);

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

        services.RegisterPolicy<ReprocessorRegistrationSession>(configuration);

    }

    private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GlobalVariables>(configuration);
        services.Configure<ExternalUrlOptions>(configuration.GetSection(ExternalUrlOptions.ConfigSection));
        services.Configure<CookieOptions>(configuration.GetSection(CookieOptions.ConfigSection));
        services.Configure<MsalOptions>(configuration.GetSection(MsalOptions.ConfigSection));
        services.Configure<EprPrnFacadeApiOptions>(configuration.GetSection(EprPrnFacadeApiOptions.ConfigSection));
        services.Configure<HttpClientOptions>(configuration.GetSection(HttpClientOptions.ConfigSection));
        services.Configure<FrontEndAccountCreationOptions>(configuration.GetSection(FrontEndAccountCreationOptions.ConfigSection));
        services.Configure<AccountsFacadeApiOptions>(configuration.GetSection(AccountsFacadeApiOptions.ConfigSection));
        services.Configure<LinksConfig>(configuration.GetSection("Links"));
        services.Configure<ModuleOptions>(configuration.GetSection(ModuleOptions.ConfigSection));
    }

    private static void RegisterServices(IServiceCollection services, IHostEnvironment env)
    {
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<ISaveAndContinueService, SaveAndContinueService>();
        services.AddScoped<ISessionManager<ReprocessorRegistrationSession>, SessionManager<ReprocessorRegistrationSession>>();
        services.AddScoped<ISessionManager<ExporterRegistrationSession>, SessionManager<ExporterRegistrationSession>>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddTransient<UserDataCheckerMiddleware>();
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IEprFacadeServiceApiClient, EprFacadeServiceApiClient>();       
        services.AddScoped<IAccreditationService, AccreditationService>();
        services.AddScoped<IPostcodeLookupService, PostcodeLookupService>();
        services.AddScoped<IReprocessorService, ReprocessorService>();

        services.AddScoped<IRegistrationMaterialService, RegistrationMaterialService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddScoped<IMaterialService, MaterialService>();
        services.AddScoped<IPostcodeLookupService, PostcodeLookupService>();
        services.AddScoped<IRequestMapper, RequestMapper>();
        services.AddScoped<IOrganisationAccessor, OrganisationAccessor>();
        services.AddScoped<IExporterRegistrationService, ExporterRegistrationService>();

        services.AddScoped(typeof(IModelFactory<>), typeof(ModelFactory<>));
    }

    private static void RegisterHttpClients(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IAccountServiceApiClient, AccountServiceApiClient>((sp, client) =>
        {
            var facadeApiOptions = sp.GetRequiredService<IOptions<AccountsFacadeApiOptions>>().Value;
            var httpClientOptions = sp.GetRequiredService<IOptions<HttpClientOptions>>().Value;

            client.BaseAddress = new Uri(facadeApiOptions.BaseEndpoint);
            client.Timeout = TimeSpan.FromSeconds(httpClientOptions.TimeoutSeconds);
        });

        services.AddHttpClient<IEprFacadeServiceApiClient, EprFacadeServiceApiClient>((sp, client) =>
        {
            var facadeApiOptions = sp.GetRequiredService<IOptions<EprPrnFacadeApiOptions>>().Value;
            var httpClientOptions = sp.GetRequiredService<IOptions<HttpClientOptions>>().Value;

            client.BaseAddress = new Uri(facadeApiOptions.BaseEndpoint);
            client.Timeout = TimeSpan.FromSeconds(httpClientOptions.TimeoutSeconds);
        });

        services.AddHttpClient<IPostcodeLookupApiClient, PostcodeLookupApiClient>((sp, client) =>
        {
            var facadeApiOptions = sp.GetRequiredService<IOptions<AccountsFacadeApiOptions>>().Value;
            var httpClientOptions = sp.GetRequiredService<IOptions<HttpClientOptions>>().Value;

            client.BaseAddress = new Uri(facadeApiOptions.BaseEndpoint);
            client.Timeout = TimeSpan.FromSeconds(httpClientOptions.TimeoutSeconds);
        });
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
                options.RequestCultureProviders =
                [
	                new SessionRequestCultureProvider()
                ];
            });
    }

    private static void ConfigureSession(IServiceCollection services, IConfiguration configuration)
    {
	    var useLocalSession = configuration.GetValue<bool>("UseLocalSession");
        
	    if (useLocalSession)
	    {
		    services.AddDistributedMemoryCache();
	    }
	    else
	    {
		    var redisConnection = configuration.GetConnectionString("REDIS_CONNECTION");
		    var redisInstanceName = configuration.GetValue<string>("RedisInstanceName");
		    var redisMultiplexer = ConnectionMultiplexer.Connect(redisConnection);
		    
		    services.AddDataProtection()
			    .SetApplicationName("EprPrn")
			    .PersistKeysToStackExchangeRedis(redisMultiplexer, "DataProtection-Keys");
		    
		    services.AddStackExchangeRedisCache(options =>
		    {
			    options.Configuration = redisConnection;
			    options.InstanceName = redisInstanceName;
		    });
	    }
        
	    var sessionCookieName = configuration.GetValue<string>("CookieOptions:SessionCookieName");
	    var sessionIdleTimeout = TimeSpan.FromMinutes(configuration.GetValue<int>("SessionIdleTimeOutMinutes"));
        
	    services.AddSession(options =>
	    {
		    options.Cookie.Name = sessionCookieName;
		    options.IdleTimeout = sessionIdleTimeout;
		    options.Cookie.IsEssential = true;
		    options.Cookie.HttpOnly = true;
		    options.Cookie.SameSite = SameSiteMode.Strict;
		    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
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
                    options.Events.OnRemoteFailure = context =>
                    {
                        var telemetry = context.HttpContext.RequestServices.GetService<TelemetryClient>();
                        telemetry?.TrackException(context.Failure);
                        telemetry?.TrackTrace("OIDC Remote Failure: " + context.Failure?.Message);
                        return Task.CompletedTask;
                    };

                    options.Events.OnAuthenticationFailed = context =>
                    {
                        var telemetry = context.HttpContext.RequestServices.GetService<TelemetryClient>();
                        telemetry?.TrackException(context.Exception);
                        telemetry?.TrackTrace("OIDC Auth Failure: " + context.Exception?.Message);
                        return Task.CompletedTask;
                    };

                    options.Events.OnTokenValidated = context =>
                    {
                        var telemetry = context.HttpContext.RequestServices.GetService<TelemetryClient>();

                        var userId = context.Principal?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
                        var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;

                        telemetry?.TrackEvent("UserLoginSuccess", new Dictionary<string, string>
                        {
                            { "UserId", userId ?? "unknown" },
                            { "Email", email ?? "unknown" },
                            { "Timestamp", DateTime.UtcNow.ToString("o") }
                        });

                        return Task.CompletedTask;
                    };
                    
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