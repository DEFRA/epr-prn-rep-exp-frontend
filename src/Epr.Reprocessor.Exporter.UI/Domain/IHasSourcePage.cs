namespace Epr.Reprocessor.Exporter.UI.Domain;

/// <summary>
/// Defines a common interface used to denote where a type wants to define a source page property.
/// </summary>
/// <typeparam name="T">The generic type parameter this is being implemented for.</typeparam>
public interface IHasSourcePage<out T> where T : class, new()
{
    /// <summary>
    /// The source page.
    /// </summary>
    public string? SourcePage { get; set; }

    /// <summary>
    /// Set the source page for the current instance, used for navigation purposes.
    /// </summary>
    /// <param name="page">The source page to set.</param>
    /// <returns>This instance.</returns>
    public T SetSourcePage(string page);
}