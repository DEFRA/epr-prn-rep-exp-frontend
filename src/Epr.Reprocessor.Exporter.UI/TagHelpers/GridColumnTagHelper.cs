using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

/// <summary>
/// Defines a tag helper that renders a <div></div> html tag that generates a row within the GDS grid system.
/// </summary>
[HtmlTargetElement(TagName, TagStructure = TagStructure.NormalOrSelfClosing, ParentTag = GridRowTagHelper.TagName, Attributes = WidthAttributeName)]
public class GridColumnTagHelper : TagHelper
{
    private const string WidthAttributeName = "width";

    /// <summary>
    /// The defined width to set the container to.
    /// </summary>
    [HtmlAttributeName(WidthAttributeName)]
    public CommonColumnWidths ColumnWidth { get; set; }

    /// <summary>
    /// The tag name used in the markup to create this element.
    /// </summary>
    public const string TagName = "grid-column";

    /// <inheritdoc/>>.
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        output.TagName = "div";
        var widthCssClass = string.Empty;

        if (ColumnWidth is CommonColumnWidths.OneThird)
        {
            widthCssClass = "govuk-grid-column-one-third";
        }
        else if (ColumnWidth is CommonColumnWidths.TwoThirds)
        {
            widthCssClass = "govuk-grid-column-two-thirds";
        }
        else if (ColumnWidth is CommonColumnWidths.Full)
        {
            widthCssClass = "govuk-grid-column-full";
        }

        output.Attributes.SetAttribute("class", widthCssClass);
    }
}