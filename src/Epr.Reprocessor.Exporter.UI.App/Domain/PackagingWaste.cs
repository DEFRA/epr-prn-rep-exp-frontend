using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.Domain;

/// <summary>
/// Represents details of the materials that form part of the packaging waste that is to be recycled.
/// <remarks>2nd Section.</remarks>
/// </summary>
[ExcludeFromCodeCoverage]
public class PackagingWaste
{
    /// <summary>
    /// Collection of all available materials that can be selected.
    /// </summary>
    public List<Material> AllMaterials { get; set; } = new();

    /// <summary>
    /// Collection of materials that have been selected.
    /// </summary>
    public List<Material> SelectedMaterials { get; set; } = new();

    /// <summary>
    /// Determines the next material that is eligible to be applied for in the registration application based on the next material in the list in alphabetical order that has not been applied for yet.
    /// </summary>
    public Material? CurrentMaterialApplyingFor => SelectedMaterials.OrderBy(o => o.Name).FirstOrDefault(o => o.Applied is false);

    /// <summary>
    /// Sets the applicable materials for the packaging waste that is to be recycled.
    /// </summary>
    /// <param name="materials">Collection of materials to set.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetApplicableMaterials(IEnumerable<MaterialDto> materials)
    {
        AllMaterials = materials.Select(o => new Material{Name = o.Name}).ToList();

        return this;
    }

    /// <summary>
    /// Sets the applicable materials for the packaging waste that is to be recycled.
    /// </summary>
    /// <param name="materials">Collection of materials to set.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetSelectedMaterials(IEnumerable<string> materials)
    {
        SelectedMaterials = materials.Select(o => new Material { Name = Enum.Parse<MaterialItem>(o) }).ToList();

        return this;
    }

    /// <summary>
    /// Sets the specified material to applied.
    /// </summary>
    /// <param name="material">The material to set to applied.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetMaterialAsApplied(MaterialItem material)
    {
        SelectedMaterials.Single(o => o.Name == material).Applied = true;

        return this;
    }
}