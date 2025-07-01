namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

public class OverseasAddressRequestDto
{
    public Guid? RegistrationMaterialId { get; set; }
    public List<OverseasAddressDto>? OverseasAddresses { get; set; }

}
