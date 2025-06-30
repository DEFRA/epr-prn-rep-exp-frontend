namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration.Exporter;

[ExcludeFromCodeCoverage]
public class BaselConventionAndOecdCodesViewModel
{
    public required string OrganisationName { get; set; }
    public required string AddressLine1 { get; set; }
    public string? MaterialName { get; set; }
    public List<OverseasAddressWasteCodesViewModel> OecdCodes { get; set; }
}