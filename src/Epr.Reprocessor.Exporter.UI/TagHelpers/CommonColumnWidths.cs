namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines common govuk grid column widths that can be used to organise the layout of a page.
/// </summary>
public enum CommonColumnWidths
{
    /// <summary>
    /// No width is set.
    /// </summary>
    None = 0,

    /// <summary>
    /// Set the column to use one third width of the entire grid row.
    /// </summary>
    OneThird = 1,

    /// <summary>
    /// Set the column to use two thirds width of the entire grid row.
    /// </summary>
    TwoThirds = 2,

    /// <summary>
    /// Set the column to use the entire available space for the grid row.
    /// </summary>
    Full = 3
}