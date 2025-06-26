namespace Epr.Reprocessor.Exporter.UI.ViewModels.Shared;

/// <summary>
/// Defines a summary list model to define the contents of a summary list.
/// </summary>
public record SummaryListModel
{
    /// <summary>
    /// The summary list rows to create.
    /// </summary>
    public List<SummaryListRowModel> Rows { get; set; } = null!;
}

/// <summary>
/// Defines a summary list row to be rendered.
/// </summary>
public record SummaryListRowModel
{
    /// <summary>
    /// The name of the field of the summary row, this is the <dt></dt> element.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// The value of the field of the summary row, this is the <dd></dd> element.
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    /// Optional change link url if a change link is required, also rendered as a <dd></dd>
    /// </summary>
    public string? ChangeLinkUrl { get; set; }

    /// <summary>
    /// If a change link url is defined then we should define an accessible text to be used that forms part of the hidden view but is used by screen readers.
    /// Especially if there are multiple change links on a screen it is good practise to add a value for this field to ensure an accessible text is set to let users
    /// know what the change link is for, especially helpful for users with accessibility needs.
    /// </summary>
    public string? ChangeLinkHiddenAccessibleText { get; set; }
}