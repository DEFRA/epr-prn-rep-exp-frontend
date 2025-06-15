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

    /// <summary>
    /// Maps a domain list of materials from the session domain to what the UI model requires.
    /// </summary>
    /// <param name="materials">The materials to map.</param>
    /// <returns>The mapped materials.</returns>
    public List<SelectListItem> MapMaterialsFromDomain(IList<Material> materials)
    {
        var mapped = materials.Select(o => new Material
        {
            Name = o.Name
        }).ToList();

        foreach (var material in mapped.Select(o => o.Name))
        {
            Materials.Add(new()
            {
                Value = material.ToString(),
                Text = material.GetDisplayName()
            });
        }

        return Materials;
    }

    /// <summary>
    /// Maps a service list of materials to what the UI requires.
    /// </summary>
    /// <param name="materials">The materials to map.</param>
    /// <returns>The mapped materials.</returns>
    public List<SelectListItem> MapMaterialsFromService(IList<MaterialDto> materials)
    {
        var mapped = materials.Select(o => new Material
        {
            Name = o.Name
        }).ToList();

        foreach (var material in mapped.Select(o => o.Name))
        {
            Materials.Add(new()
            {
                Value = material.ToString(),
                Text = material.GetDisplayName()
            });
        }

        return Materials;
    }
}