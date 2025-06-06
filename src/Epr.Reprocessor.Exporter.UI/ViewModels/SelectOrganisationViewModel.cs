namespace Epr.Reprocessor.Exporter.UI.ViewModels;

public class SelectOrganisationViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<OrganisationViewModel> Organisations { get; set; } = [];
}

public class OrganisationViewModel
{
    public string OrganisationName { get; set; }
    public string OrganisationNumber { get; set; }
}

