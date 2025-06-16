using Epr.Reprocessor.Exporter.UI.Enums;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers
{

    [HtmlTargetElement("*", Attributes = RegistrationStatusAttributeName)]
    public class RegistrationStatusColourClassTagHelper : TagHelper
    {
        private const string RegistrationStatusAttributeName = "asp-registration-status-colour";

        /// <summary>
        /// This will add a govuk-tag--[colour] css class to the then end of the class attribute for your element based upon the registration status provided.
        /// </summary>
        [HtmlAttributeName(RegistrationStatusAttributeName)]
       
        public ModelExpression RegStatus { get; set; }
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!Enum.TryParse(typeof(RegistrationStatus), RegStatus.Model?.ToString(), out var status))
                return;

            // Perform a switch on the RegistrationStatus enum
            var classToAppend = status switch
            {
                RegistrationStatus.InProgress => "govuk-tag--blue",
                RegistrationStatus.Completed => "govuk-tag--blue",
                RegistrationStatus.Submitted => "govuk-tag--yellow",
                RegistrationStatus.RegulatorReviewing => "govuk-tag--pink",
                RegistrationStatus.Queried => "govuk-tag--purple",
                RegistrationStatus.Updated => string.Empty, //default dark blue
                RegistrationStatus.Refused => "govuk-tag--red",
                RegistrationStatus.Granted => "govuk-tag--green",
                RegistrationStatus.RenewalInProgress => "govuk-tag--blue",
                RegistrationStatus.RenewalSubmitted => "govuk-tag--yellow",
                RegistrationStatus.RenewalQueried => "govuk-tag--purple",
                RegistrationStatus.Suspended => "govuk-tag--red",
                RegistrationStatus.Cancelled => "govuk-tag--grey",
                RegistrationStatus.NeedsToBeRenewed => string.Empty,

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