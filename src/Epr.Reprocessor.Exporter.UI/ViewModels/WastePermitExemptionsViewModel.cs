using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

/// <summary>
/// Model for the waste permit exemptions page.
/// </summary>
[ExcludeFromCodeCoverage]
public class WastePermitExemptionsViewModel
{
	/// <summary>
	/// Collection of applicable materials.
	/// </summary>
	public List<SelectListItem> Materials { get; set; } = [];

    /// <summary>
    /// Collection of selected materials that the site has a permit or exemption to accept and recycle.
    /// </summary>
    [Required(ErrorMessage = "Select all the material categories the site has a permit or exemption to accept and recycle")]
	public List<string> SelectedMaterials { get; set; } = [];
}