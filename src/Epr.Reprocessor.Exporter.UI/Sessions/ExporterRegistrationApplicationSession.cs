using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Sessions;


[ExcludeFromCodeCoverage]
public class ExporterRegistrationApplicationSession
{
    public Guid? RegistrationMaterialId { get; set; }
    public OverseasReprocessingSites? OverseasReprocessingSites { get; set; }
}