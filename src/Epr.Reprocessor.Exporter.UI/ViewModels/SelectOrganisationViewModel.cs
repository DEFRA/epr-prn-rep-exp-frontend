using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Home;
namespace Epr.Reprocessor.Exporter.UI.ViewModels;

[ExcludeFromCodeCoverage]
public class SelectOrganisationViewModel
{
    public List<OrganisationViewModel> Organisations { get; set; } = [];

	[Required(ErrorMessageResourceName = "SelectOrganisation_ErrorMessage", ErrorMessageResourceType =typeof(SelectOrganisation))]
	public Guid? SelectedOrganisationId { get; set; }
}