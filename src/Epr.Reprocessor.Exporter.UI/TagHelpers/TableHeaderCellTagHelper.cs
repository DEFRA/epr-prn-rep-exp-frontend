using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper to render a GDS table header cell with correct styling.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableRowTagHelper.TagName)]
public class TableHeaderCellTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-table__header";
    private const string DefaultScope = "col";

    /// <summary>
    /// The name of the tag that is to be used that renders this tag helper.
    /// </summary>
    public const string TagName = "table-header";

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "th";
        output.Attributes.Add("class", DefaultCssClass);
        output.Attributes.Add("scope", DefaultScope);
    }
}