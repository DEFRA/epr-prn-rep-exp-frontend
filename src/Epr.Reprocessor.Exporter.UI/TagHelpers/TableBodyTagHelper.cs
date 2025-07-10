using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper to render a GDS table body with correct styling.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableTagHelper.TagName)]
public class TableBodyTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-table__body";

    /// <summary>
    /// The name of the tag that is to be used that renders this tag helper.
    /// </summary>
    public const string TagName = "table-body";

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "tbody";
        output.Attributes.Add("class", DefaultCssClass);
    }
}