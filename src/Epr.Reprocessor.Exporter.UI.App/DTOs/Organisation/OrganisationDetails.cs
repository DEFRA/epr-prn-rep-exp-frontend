namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Organisation;

public class OrganisationDetails
{
    public string OrganisationName { get; set; }
    public string TradingName { get; set; }
    public string OrganisationType { get; set; }
    public string CompaniesHouseNumber { get; set; }
    public string RegisteredAddress { get; set; }
    public List<OrganisationPerson> Persons { get; set; }
}
