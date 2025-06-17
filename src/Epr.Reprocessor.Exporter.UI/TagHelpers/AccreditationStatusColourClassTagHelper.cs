using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers
{

    [HtmlTargetElement("*", Attributes = AccreditationStatusAttributeName)]
    public class AccreditationStatusColourClassTagHelper : TagHelper
    {
        private const string AccreditationStatusAttributeName = "asp-accreditation-status-colour";

        /// <summary>
        /// This will add a govuk-tag--[colour] css class to the then end of the class attribute for your element based upon the registration status provided.
        /// </summary>
        [HtmlAttributeName(AccreditationStatusAttributeName)]
       
        public ModelExpression AccredStatus { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Enum.TryParse(typeof(Enums.AccreditationStatus), AccredStatus.Model?.ToString(), out var status))
                return;

            // Perform a switch on the AccreditationStatus enum
            var classToAppend = status switch
            {
                //AccreditationStatus.InProgress => string.Empty,
                Enums.AccreditationStatus.Started => "govuk-tag--blue",
                Enums.AccreditationStatus.Submitted => "govuk-tag--turquoise",
                Enums.AccreditationStatus.Queried => "govuk-tag--purple",
                Enums.AccreditationStatus.Updated => string.Empty,
                Enums.AccreditationStatus.Refused => "govuk-tag--red",
                Enums.AccreditationStatus.Granted => "govuk-tag--green",
                Enums.AccreditationStatus.Accepted => "govuk-tag--green",
                Enums.AccreditationStatus.NotAccredited => "govuk-tag--grey",
                Enums.AccreditationStatus.Suspended => "govuk-tag--red",
                Enums.AccreditationStatus.Cancelled => string.Empty,
                _ => string.Empty
            };

            // Append the class to the existing class attribute
            if (!string.IsNullOrEmpty(classToAppend))
            {
                output.Attributes.SetAttribute("class", $"{output.Attributes["class"]?.Value} {classToAppend}".Trim());
            }
        }
    }
}