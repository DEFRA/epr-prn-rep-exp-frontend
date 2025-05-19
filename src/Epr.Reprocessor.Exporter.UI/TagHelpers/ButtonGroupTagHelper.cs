using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper that renders a <div></div> html tag that generates a button group with associated GDS styling.
/// </summary>
[HtmlTargetElement(TagName, TagStructure = TagStructure.NormalOrSelfClosing)]
public class ButtonGroupTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-button-group";

    /// <summary>
    /// The tag name used in the markup to create this element.
    /// </summary>
    public const string TagName = "button-group";

    /// <inheritdoc/>>.
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        output.TagName = "div";
        output.Attributes.SetAttribute("class", DefaultCssClass);
    }
}