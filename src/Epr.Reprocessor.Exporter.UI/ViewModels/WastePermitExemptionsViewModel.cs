using Epr.Reprocessor.Exporter.UI.Attributes.Validations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

/// <summary>
/// Represents the model for the waste permit exemptions page.
/// </summary>
[ExcludeFromCodeCoverage]
public class WastePermitExemptionsViewModel
{
	/// <summary>
	/// List of materials displayed.
	/// </summary>
	public List<SelectListItem> Materials { get; set; } = [];

	/// <summary>
	/// List of selected materials.
	/// </summary>
    [ListCannotBeEmpty<string>(ErrorMessageResourceType = typeof(WastePermitExemptions), ErrorMessageResourceName = "required_error_message")]
	public List<string> SelectedMaterials { get; set; } = [];
}