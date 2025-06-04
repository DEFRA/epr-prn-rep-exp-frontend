namespace Epr.Reprocessor.Exporter.UI.Domain;

/// <summary>
/// Represents details of the materials that form part of the packaging waste that is to be recycled.
/// <remarks>2nd Section.</remarks>
/// </summary>
[ExcludeFromCodeCoverage]
public class PackagingWaste
{
    /// <summary>
    /// Collection of materials that are to be recycled as part of the packaging waste.
    /// </summary>
    public IList<Material> Materials { get; set; } = new List<Material>();

    /// <summary>
    /// Stores the current material that is being applied for.
    /// </summary>
    public Material? CurrentMaterialApplyingFor { get; set; }

    /// <summary>
    /// Determines the next material that is eligible to be applied for in the registration application based on the next material in the list in alphabetical order that has not been applied for yet.
    /// </summary>
    public Material? NextEligibleMaterialToApplyFor => Materials.OrderBy(o => o.Name).FirstOrDefault(o => o.Applied is false);
}