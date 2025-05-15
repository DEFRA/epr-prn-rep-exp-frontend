using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper that renders a <div></div> html tag that generates a row within the GDS grid system.
/// </summary>
[HtmlTargetElement(TagName, TagStructure = TagStructure.NormalOrSelfClosing)]
public class GridRowTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-grid-row";

    /// <summary>
    /// The tag name used in the markup to create this element.
    /// </summary>
    public const string TagName = "grid-row";

    /// <inheritdoc/>>.
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        output.TagName = "div";
        output.Attributes.SetAttribute("class", DefaultCssClass);
    }
}