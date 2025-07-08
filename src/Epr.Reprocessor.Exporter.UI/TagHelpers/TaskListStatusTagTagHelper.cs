using Epr.Reprocessor.Exporter.UI.App.Enums.Registration;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Epr.Reprocessor.Exporter.UI.TagHelpers;

[HtmlTargetElement(TagName, Attributes = StatusAttribute)]
public class TaskListStatusTagTagHelper : TagHelper
{
    private const string BaseCssClass = "govuk-tag";
    private const string StatusAttribute = "asp-task-status";
    private const string TagName = "task-list-status";

    [HtmlAttributeName("asp-task-status")]
    public ApplicantRegistrationTaskStatus Status { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "strong";
        
        var additionalClass = Status switch
        {
            ApplicantRegistrationTaskStatus.CannotStartYet => "govuk-tag--grey",
            ApplicantRegistrationTaskStatus.NotStarted => "govuk-tag--grey",
            ApplicantRegistrationTaskStatus.Started => "govuk-tag--blue",
            ApplicantRegistrationTaskStatus.Completed => "govuk-tag--green",
            _ => throw new ArgumentOutOfRangeException($@"Unknown task status: {Status}. Please ensure the status is defined in the TaskStatus enum.")
        };

        output.Attributes.Add("class", $"{BaseCssClass} {additionalClass}");
    }
}