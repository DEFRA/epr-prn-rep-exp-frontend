using System.Security.Claims;
using EPR.Common.Authorization.Models;

namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

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