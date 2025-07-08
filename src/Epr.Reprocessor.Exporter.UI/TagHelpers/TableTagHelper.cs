using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper to render a GDS table with correct styling.
/// </summary>
[HtmlTargetElement(TagName)]
public class TableTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-table";

    /// <summary>
    /// The name of the tag that is to be used that renders this tag helper.
    /// </summary>
    public const string TagName = "gov-table";

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "table";
        output.Attributes.Add("class", DefaultCssClass);
    }
}