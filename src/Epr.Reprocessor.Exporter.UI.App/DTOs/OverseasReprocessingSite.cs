namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

[ExcludeFromCodeCoverage]
public class OverseasReprocessingSite
{
    public string OrganisationName { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string AddressLine3 { get; set; }

    public string Address => $"{AddressLine1}, {AddressLine2}, {AddressLine3}";

    public bool EvidenceUploaded { get; set; }
}