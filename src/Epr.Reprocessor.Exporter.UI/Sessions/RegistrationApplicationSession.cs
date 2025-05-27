using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.Domain;

namespace Epr.Reprocessor.Exporter.UI.Sessions;

/// <summary>
/// Represents a session for the registration application.
/// </summary>
[ExcludeFromCodeCoverage]
public class RegistrationApplicationSession
{
    /// <summary>
    /// The site's location nation id
    /// </summary>
    public UkNation? SiteLocationNationId { get; set; }
    /// <summary>
    /// Details of the reprocessing site.
    /// </summary>
    public ReprocessingSite? ReprocessingSite { get; set; } = new();
}