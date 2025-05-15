using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper that renders a <main></main> html tag with associated GDS styling.
/// </summary>
[HtmlTargetElement(TagName, TagStructure = TagStructure.NormalOrSelfClosing)]
public class MainWrapperTagHelper : TagHelper
{
    private const string DefaultCssClass = "govuk-main-wrapper";

    /// <summary>
    /// The tag name used in the markup to create this element.
    /// </summary>
    public const string TagName = "main-wrapper";

    /// <summary>
    /// Specifies whether to include the additional top padding class that is commonplace within the frontend.
    /// </summary>
    /// <remarks>Default value is true as thats the common value at the moment.</remarks>
    [HtmlAttributeName("include-top-padding")]
    public bool IncludeAdditionalTopPadding { get; set; } = true;

    /// <inheritdoc/>>.
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);
        
        output.TagName = "main";
        output.Attributes.SetAttribute("role", "main");
        output.Attributes.SetAttribute("id", "main-content");

        if (IncludeAdditionalTopPadding)
        {
            output.Attributes.SetAttribute("class", $"{DefaultCssClass} govuk-!-padding-top-4");
        }
        else
        {
            output.Attributes.SetAttribute("class", DefaultCssClass);
        }
    }
}