using System.Security.Claims;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

/// <summary>
/// Implementation for <see cref="IOrganisationAccessor"/>.
/// </summary>
/// <param name="httpContextAccessor">Provides access to the HttpContext.</param>
[ExcludeFromCodeCoverage]
public class OrganisationAccessor(IHttpContextAccessor httpContextAccessor) : IOrganisationAccessor
{
    /// <inheritdoc />>
    public ClaimsPrincipal? OrganisationUser => httpContextAccessor.HttpContext?.User;

    /// <inheritdoc />>
    public IList<Organisation> Organisations => httpContextAccessor.HttpContext?.GetUserData().Organisations ?? [];
}