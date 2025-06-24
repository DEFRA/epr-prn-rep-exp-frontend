using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Sessions;

/// <summary>
/// Represents a session for the registration application.
/// </summary>
[ExcludeFromCodeCoverage]
public class ExporterRegistrationApplicationSession
{
    public OverseasReprocessingSites? OverseasReprocessingSites { get; set; }
}