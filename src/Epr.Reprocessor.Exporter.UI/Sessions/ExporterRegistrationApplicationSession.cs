using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Sessions;


[ExcludeFromCodeCoverage]
public class ExporterRegistrationApplicationSession
{
    public OverseasReprocessingSites? OverseasReprocessingSites { get; set; }
    public InterimSites InterimSites { get; set; }
}