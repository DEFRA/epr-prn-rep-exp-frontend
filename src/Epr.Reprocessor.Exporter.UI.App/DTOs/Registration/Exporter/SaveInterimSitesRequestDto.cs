using Epr.Reprocessor.Exporter.UI.App.Domain.Exporter;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;
[ExcludeFromCodeCoverage]
public class SaveInterimSitesRequestDto
{
    public Guid RegistrationMaterialId { get; set; }
    public required List<OverseasMaterialReprocessingSiteDto> OverseasMaterialReprocessingSites { get; set; }
}
