using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper that renders a <div></div> html tag that generates a container for radio buttons.
/// </summary>
[HtmlTargetElement(TagName, TagStructure = TagStructure.NormalOrSelfClosing)]
public class RadioButtonContainerTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-radios";
    private const string DefaultDataModule = "govuk-radios";

    /// <summary>
    /// The tag name used in the markup to create this element.
    /// </summary>
    public const string TagName = "radios-container";

    /// <inheritdoc/>>.
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        output.TagName = "div";
        output.Attributes.SetAttribute("class", DefaultCssClass);
        output.Attributes.SetAttribute("data-module", DefaultDataModule);
    }
}