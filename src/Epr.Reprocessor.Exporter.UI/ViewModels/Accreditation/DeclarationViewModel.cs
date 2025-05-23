using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class DeclarationViewModel : IValidatableObject
    {
        public int ApplicationTypeId { get; set; }

        public Guid AccreditationId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string? FullName { get; set; } = string.Empty;

        public string? JobTitle { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // There are 4 validation rules each for FullName and JobTitle, but we don't want mulitple triggering at once, which is possible when using DataAnnotations.

            var regex = new Regex("[a-zA-Z]"); // Matches at least one alphbetic character anywhere in string.
            var minLength = 3;
            var maxLength = 100;

            if (string.IsNullOrEmpty(FullName))
                yield return new ValidationResult(ViewResources.Declaration.ResourceManager.GetString("error_message_full_name_required"), new[] { nameof(FullName) });
            else if (!regex.IsMatch(FullName))
                yield return new ValidationResult(ViewResources.Declaration.ResourceManager.GetString("error_message_full_name_invalid"), new[] { nameof(FullName) });
            else if (FullName.Length < minLength)
                yield return new ValidationResult(ViewResources.Declaration.ResourceManager.GetString("error_message_full_name_too_short"), new[] { nameof(FullName) });
            else if (FullName.Length > maxLength)
                yield return new ValidationResult(ViewResources.Declaration.ResourceManager.GetString("error_message_full_name_too_long"), new[] { nameof(FullName) });

            if (string.IsNullOrEmpty(JobTitle))
                yield return new ValidationResult(ViewResources.Declaration.ResourceManager.GetString("error_message_job_title_required"), new[] { nameof(JobTitle) });
            else if (!regex.IsMatch(JobTitle))
                yield return new ValidationResult(ViewResources.Declaration.ResourceManager.GetString("error_message_job_title_invalid"), new[] { nameof(JobTitle) });
            else if (JobTitle.Length < minLength)
                yield return new ValidationResult(ViewResources.Declaration.ResourceManager.GetString("error_message_job_title_too_short"), new[] { nameof(JobTitle) });
            else if (JobTitle.Length > maxLength)
                yield return new ValidationResult(ViewResources.Declaration.ResourceManager.GetString("error_message_job_title_too_long"), new[] { nameof(JobTitle) });
        }
    }
}
