using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Epr.Reprocessor.Exporter.UI.Middleware;
using Epr.Reprocessor.Exporter.UI.Validations.Registration;
using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Logging;
using CookieOptions = Epr.Reprocessor.Exporter.UI.App.Options.CookieOptions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var builderConfig = builder.Configuration;
var globalVariables = builderConfig.Get<GlobalVariables>();
var basePath = globalVariables?.BasePath;

services.AddLocalization();
services.AddFeatureManagement();

services.AddAntiforgery(opts =>
{
    var cookieOptions = builderConfig.GetSection(CookieOptions.ConfigSection).Get<CookieOptions>();
    opts.Cookie.Name = cookieOptions?.AntiForgeryCookieName;
    opts.Cookie.Path = basePath;
});

services
    .AddHttpContextAccessor()
    .RegisterWebComponents(builderConfig)
    .ConfigureMsalDistributedTokenOptions();

services
    .AddControllersWithViews(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    })
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

services.AddRazorPages();

services.Configure<ForwardedHeadersOptions>(options =>
{
    var forwardedHeadersOptions = builderConfig.GetSection("ForwardedHeaders").Get<ForwardedHeadersOptions>();

    options.ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
    options.ForwardedHostHeaderName = forwardedHeadersOptions.ForwardedHostHeaderName;
    options.OriginalHostHeaderName = forwardedHeadersOptions.OriginalHostHeaderName;
    options.AllowedHosts = forwardedHeadersOptions.AllowedHosts;
});

services.AddHealthChecks();

services.AddApplicationInsightsTelemetry()
        .AddLogging();

services.AddHsts(options =>
{
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

services.AddValidatorsFromAssemblyContaining<ManualAddressForServiceOfNoticesValidator>();

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

var app = builder.Build();

app.MapHealthChecks("/admin/health").AllowAnonymous();

app.UsePathBase(basePath);

// Add middleware to redirect requests missing the base path
app.Use(async (context, next) =>
{
    // Ensure basePath is not null or empty
    if (!string.IsNullOrEmpty(basePath) && context.Request.PathBase != basePath)
    {
        // Redirect only if the basePath is missing
        var newPath = $"{basePath}{context.Request.Path}";
        context.Response.Redirect(newPath, permanent: false);
        return;
    }
    // Proceed to the next middleware
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

//TODO: Add security middleware 
app.UseMiddleware<SecurityHeaderMiddleware>();
app.UseCookiePolicy();
app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
//TODO: Dependency on enrollment and user account setup - Currently in progress
app.UseAuthentication();
app.UseAuthorization();
//TODO: Check if UserDataCheckerMiddleware required
//app.UseMiddleware<UserDataCheckerMiddleware>();
//TODO: Check if JourneyAccessCheckerMiddleware required
//app.UseMiddleware<JourneyAccessCheckerMiddleware>();
//TODO: Check if data AnalyticsCookieMiddleware required
app.UseMiddleware<AnalyticsCookieMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapControllers();
app.UseRequestLocalization();

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

app.Run();
