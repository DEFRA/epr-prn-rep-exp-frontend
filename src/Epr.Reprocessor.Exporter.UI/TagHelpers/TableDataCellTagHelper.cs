using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper to render a GDS table data cell with correct styling.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableRowTagHelper.TagName)]
public class TableDataCellTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-table__cell";

    /// <summary>
    /// The name of the tag that is to be used that renders this tag helper.
    /// </summary>
    public const string TagName = "table-data";

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "td";
        output.Attributes.Add("class", DefaultCssClass);
    }
}