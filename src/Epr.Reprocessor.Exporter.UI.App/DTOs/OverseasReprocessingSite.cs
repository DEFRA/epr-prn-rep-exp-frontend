namespace Epr.Reprocessor.Exporter.UI.App.DTOs;

[ExcludeFromCodeCoverage]
public class OverseasReprocessingSite
{
    private string organisationNameAndAddress = string.Empty;

    public string OrganisationName { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string AddressLine3 { get; set; }

    public string NameAndAddress
    {
        get => organisationNameAndAddress;
        set
        {
            organisationNameAndAddress = value;
            var parts = organisationNameAndAddress.Split(',', StringSplitOptions.TrimEntries);

            if (parts.Length > 0)
                OrganisationName = parts[0];
            if (parts.Length > 1)
                AddressLine1 = parts[1];
            if (parts.Length > 2)
                AddressLine2 = parts[2];
            if (parts.Length > 3)
                AddressLine3 = parts[3] + (parts.Length > 4 ? $", {parts[4]}" : string.Empty);
        }
    }

    public string Country { get; set; }

    public string Address => $"{AddressLine1}, {AddressLine2}, {AddressLine3}";

    public bool EvidenceUploaded { get; set; }

    public bool SiteCheckedForConditionFulfilment { get; set; }
}