using System.Security.Claims;

namespace Epr.Reprocessor.Exporter.UI.Controllers;

/// <summary>
/// Defines a contract to access the organisation data stored in the HttpContext.
/// </summary>
public interface IOrganisationAccessor
{
    /// <summary>
    /// The user associated with the organisation.
    /// </summary>
    public ClaimsPrincipal? OrganisationUser { get; }

    /// <summary>
    /// Collection of organisations associated with the user.
    /// </summary>
    public IList<Organisation> Organisations { get; }
}