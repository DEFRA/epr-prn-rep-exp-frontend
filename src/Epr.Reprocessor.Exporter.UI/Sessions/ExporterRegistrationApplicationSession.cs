using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Sessions;


[ExcludeFromCodeCoverage]
public class ExporterRegistrationApplicationSession
{
    public OverseasReprocessingSites? OverseasReprocessingSites { get; set; }

    public bool? AddOverseasSiteAccepted { get; set; }
}