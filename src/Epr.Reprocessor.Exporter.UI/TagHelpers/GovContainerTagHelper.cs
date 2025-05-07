using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper that renders a <div></div> html container tag that wraps all the main body.
/// </summary>
[HtmlTargetElement(TagName, TagStructure = TagStructure.NormalOrSelfClosing)]
public class GovContainerTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-width-container";

    /// <summary>
    /// The tag name used in the markup to create this element.
    /// </summary>
    public const string TagName = "gov-container";

    /// <inheritdoc/>>.
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        output.TagName = "div";
        output.Attributes.SetAttribute("class", DefaultCssClass);
    }
}