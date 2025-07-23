namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class OverseasAddressWasteCodesDto
{
    public Guid? ExternalId { get; set; }
    public required string CodeName { get; set; }
}