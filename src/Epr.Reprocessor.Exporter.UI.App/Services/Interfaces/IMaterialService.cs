namespace Epr.Reprocessor.Exporter.UI.App.Services.Interfaces;

/// <summary>
/// Defines an Api to manage materials.
/// </summary>
public interface IMaterialService
{
    /// <summary>
    /// Retrieves all materials that can be applied for.
    /// </summary>
    /// <returns>Collection of found materials.</returns>
    Task<List<MaterialLookupDto>> GetAllMaterialsAsync();
}