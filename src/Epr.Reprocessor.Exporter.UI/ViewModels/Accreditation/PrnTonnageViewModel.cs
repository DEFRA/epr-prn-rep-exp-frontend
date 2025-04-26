using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Humanizer.Localisation;
using NuGet.Protocol.Core.Types;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation
{
    [ExcludeFromCodeCoverage]
    public class PrnTonnageViewModel : IValidatableObject
    {
        public string Subject { get; set; } = "PRN";
        public string? Action { get; set; }

        public string MaterialName { get; set; } = string.Empty;

        public string? PrnTonnage { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // The validation error needs to include the word "PRN" or "PERN" from the Subject property. Can't be done using annotation attributes.
            if (string.IsNullOrEmpty(PrnTonnage))
            {
                yield return new ValidationResult(
                    String.Format(ViewResources.PrnTonnage.ResourceManager.GetString("error_message"), Subject),
                    new[] { nameof(PrnTonnage) });
            }
        }
    }
}
