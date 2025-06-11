using Epr.Reprocessor.Exporter.UI.App.Options;
using Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;
using Epr.Reprocessor.Exporter.UI.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;

namespace Epr.Reprocessor.Exporter.UI.Middleware;

public class UserDataCheckerMiddleware : IMiddleware
{
    private readonly IUserAccountService _userAccountService;
    private readonly ILogger<UserDataCheckerMiddleware> _logger;
    private readonly IOptions<ModuleOptions> _module;
    private readonly FrontEndAccountCreationOptions _frontEndAccountCreationOptions;

    public UserDataCheckerMiddleware(
        IOptions<FrontEndAccountCreationOptions> frontendAccountCreationOptions,
        IUserAccountService userAccountService,
        ILogger<UserDataCheckerMiddleware> logger,
        IOptions<ModuleOptions> module)
    {
        _frontEndAccountCreationOptions = frontendAccountCreationOptions.Value;
        _userAccountService = userAccountService;
        _logger = logger;
        _module = module;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var anonControllers = new List<string> { "Privacy", "Cookies", "Culture", "Account" };
        var controllerName = GetControllerName(context);

        if (!anonControllers.Contains(controllerName) && context.User.Identity is { IsAuthenticated: true } && (context.User.TryGetUserData() is null
            || context.User.GetOrganisationId() is null))
        {
            var userAccount = await _userAccountService.GetUserAccount(_module.Value.ServiceKey);

            if (userAccount is null)
            {
                _logger.LogInformation("User authenticated but account could not be found");
                context.Response.Redirect(_frontEndAccountCreationOptions.CreateUser);
                return;
            }

            var userData = new UserData
            {
                Service = userAccount.User.Service,
                ServiceRole = userAccount.User.Organisations?.FirstOrDefault()?.Enrolments.FirstOrDefault()?.ServiceRole,
                ServiceRoleId = userAccount.User.Organisations?.FirstOrDefault()?.Enrolments.FirstOrDefault()?.ServiceRoleId ?? 0,
                FirstName = userAccount.User.FirstName,
                LastName = userAccount.User.LastName,
                Email = userAccount.User.Email,
                Id = userAccount.User.Id,
                RoleInOrganisation = userAccount.User.RoleInOrganisation,
                EnrolmentStatus = userAccount.User.Organisations?.FirstOrDefault()?.Enrolments.FirstOrDefault()?.EnrolmentStatus,
                Organisations = userAccount.User.Organisations?.Select(x =>
                    new Organisation
                    {
                        Id = x.Id,
                        Name = x.OrganisationName,
                        OrganisationNumber = x.OrganisationNumber,
                        OrganisationRole = x.OrganisationRole,
                        OrganisationType = x.OrganisationType,
                        NationId = (int)x.NationId,
                        Locality = x.Locality,
                        DependentLocality = x.DependentLocality,
                        Street = x.Street,
                        Town = x.Town,
                        County = x.County,
                        Postcode = x.Postcode,
                        BuildingNumber = x.BuildingNumber,
                        BuildingName = x.BuildingName,
                        CompaniesHouseNumber = x.CompaniesHouseNumber,
                        Country = x.Country,
                        SubBuildingName = x.SubBuildingName,
                        JobTitle = x.JobTitle
                    }).ToList()
            };

            await ClaimsExtensions.UpdateUserDataClaimsAndSignInAsync(context, userData);
        }

        await next(context);
    }

    private static string GetControllerName(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint != null)
        {
            return endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()?.ControllerName ?? string.Empty;
        }

        return string.Empty;
    }
}