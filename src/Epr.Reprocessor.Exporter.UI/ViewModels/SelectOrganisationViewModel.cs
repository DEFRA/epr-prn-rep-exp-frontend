namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class SelectOrganisationViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<OrganisationViewModel> Organisations { get; set; } = [];
}