using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper to render a GDS table row with correct styling.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableBodyTagHelper.TagName)]
[HtmlTargetElement(TagName, ParentTag = TableHeadTagHelper.TagName)]
public class TableRowTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-table__row";

    /// <summary>
    /// The name of the tag that is to be used that renders this tag helper.
    /// </summary>
    public const string TagName = "table-row";

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "tr";
        output.Attributes.Add("class", DefaultCssClass);
    }
}