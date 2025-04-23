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
                RegistrationStatus.InProgress => "status-in-progress",
                RegistrationStatus.Completed => "govuk-tag--blue",
                RegistrationStatus.Submitted => "govuk-tag--turquoise",
                RegistrationStatus.RegulatorReviewing => "status-reviewing",
                RegistrationStatus.Queried => "govuk-tag--blue",
                RegistrationStatus.Updated => "status-updated",
                RegistrationStatus.Refused => "status-refused",
                RegistrationStatus.Granted => "govuk-tag--green",
                RegistrationStatus.RenewalInProgress => "status-renewal-in-progress",
                RegistrationStatus.RenewalSubmitted => "status-renewal-submitted",
                RegistrationStatus.RenewalQueried => "status-renewal-queried",
                RegistrationStatus.Suspended => "status-suspended",
                RegistrationStatus.Cancelled => "status-cancelled",
                RegistrationStatus.NeedsToBeRenewed => "status-needs-renewal",
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
