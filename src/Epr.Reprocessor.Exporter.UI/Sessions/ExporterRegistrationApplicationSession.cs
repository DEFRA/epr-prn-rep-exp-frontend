using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;
using Epr.Reprocessor.Exporter.UI.App.Domain.Registration.Exporter;

namespace Epr.Reprocessor.Exporter.UI.Sessions;


[ExcludeFromCodeCoverage]
public class ExporterRegistrationApplicationSession
{
    public Guid? RegistrationMaterialId { get; set; }
    public OverseasReprocessingSites? OverseasReprocessingSites { get; set; }
}