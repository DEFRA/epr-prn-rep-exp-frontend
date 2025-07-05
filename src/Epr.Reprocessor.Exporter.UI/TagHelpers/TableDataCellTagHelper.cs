using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper to render a GDS table data cell with correct styling.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableRowTagHelper.TagName)]
public class TableDataCellTagHelper : TagHelper
{
    private const string TagName = "table-data";
    private const string DefaultCssClass = "govuk-table__cell";

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "td";
        output.Attributes.Add("class", DefaultCssClass);
    }
}