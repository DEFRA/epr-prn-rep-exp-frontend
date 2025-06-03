using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper that renders a <div></div> html tag that generates a container for a radio button.
/// </summary>
[HtmlTargetElement(TagName, ParentTag = RadioButtonContainerTagHelper.TagName, TagStructure = TagStructure.NormalOrSelfClosing)]
public class RadioButtonItemTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-radios__item";

    /// <summary>
    /// The tag name used in the markup to create this element.
    /// </summary>
    public const string TagName = "radio-item";

    /// <inheritdoc/>>.
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        output.TagName = "div";
        output.Attributes.SetAttribute("class", DefaultCssClass);
    }
}