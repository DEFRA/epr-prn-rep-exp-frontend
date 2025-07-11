namespace Epr.Reprocessor.Exporter.UI.Services;

/// <summary>
/// Defines an Api to work out what the nation is.
/// </summary>
public interface INationAccessor
{
    /// <summary>
    /// Get the nation of the organisation or site address if they are not one and the same.
    /// </summary>
    /// <returns>The resolved nation.</returns>
    Task<UkNation?> GetNation();
}