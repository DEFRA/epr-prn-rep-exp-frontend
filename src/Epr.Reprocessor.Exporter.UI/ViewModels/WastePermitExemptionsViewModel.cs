using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.TagHelpers;

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
	public List<CheckboxItem> Materials { get; set; } = [];

    /// <summary>
    /// Collection of selected materials that the site has a permit or exemption to accept and recycle.
    /// </summary>
    [Required(ErrorMessage = "Select all the material categories the site has a permit or exemption to accept and recycle")]
	public List<string> SelectedMaterials { get; set; } = [];

    /// <summary>
    /// Maps a domain list of materials from the session domain to what the UI model requires.
    /// </summary>
    /// <param name="materials">The materials to map.</param>
    /// <returns>The mapped materials.</returns>
    public List<CheckboxItem> MapForView(IList<MaterialLookupDto> materials)
    {
        foreach (var material in materials)
        {
            Materials.Add(new()
            {
                Value = material.Name.ToString(),
                LabelText = material.DisplayText
            });
        }

        return Materials;
    }

    /// <summary>
    /// Takes in a collection of existing registration materials retrieved from the backend and ensures their corresponding checkboxes are checked on the UI.
    /// </summary>
    /// <param name="existing">The collection of existing registration materials.</param>
    /// <returns>This instance.</returns>
    public WastePermitExemptionsViewModel SetExistingMaterialsAsChecked(List<MaterialLookupDto> existing)
    {
        foreach (var item in existing)
        {
            Materials.Single(o => o.Value == item!.Name.ToString()).SetChecked();
            SelectedMaterials.Add(item.Name.ToString());
        }
        
        return this;
    }
}