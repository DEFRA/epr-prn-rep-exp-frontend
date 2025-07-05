using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper to render a GDS table caption with correct styling.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = TableTagHelper.TagName)]
public class TableCaptionTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-table__caption govuk-table__caption--m";
    private const string VisuallyHiddenCssClass = "govuk-visually-hidden";

    /// <summary>
    /// The name of the tag that is to be used that renders this tag helper.
    /// </summary>
    public const string TagName = "table-caption";

    /// <summary>
    /// Attribute that controls whether to add the visually hidden css class.
    /// </summary>
    [HtmlAttributeName("is-hidden")]
    public bool IsHidden { get; set; }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "caption";
        output.Attributes.Add("class", DefaultCssClass);

        if (IsHidden)
        {
            output.Attributes.SetAttribute("class", $"{DefaultCssClass} {VisuallyHiddenCssClass}");
        }
    }
}