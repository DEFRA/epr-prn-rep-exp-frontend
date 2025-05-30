using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;

[ExcludeFromCodeCoverage]
public class MoreDetailOnBusinessPlanViewModel: IValidatableObject
{
    public Guid AccreditationId { get; set; }
    public string Subject { get; set; }
    public int ApplicationTypeId { get; set; }
    public string FormPostRouteName { get; set; }
    public string? Action { get; set; }

    public bool ShowInfrastructure { get; set; } = false;
    public bool ShowPriceSupport { get; set; } = false;
    public bool ShowBusinessCollections { get; set; } = false;
    public bool ShowCommunications { get; set; } = false;
    public bool ShowNewMarkets { get; set; } = false;
    public bool ShowNewUses { get; set; } = false;
    public bool ShowOther { get; set; } = false;

    public string? Infrastructure { get; set; }
    public string? PriceSupport { get; set; }
    public string? BusinessCollections { get; set; }
    public string? Communications { get; set; }
    public string? NewMarkets { get; set; }
    public string? NewUses { get; set; }
    public string? Other { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Notes fields are required if the corresponding percentage field is not null and greater than zero.
        // Notes fields must contain at least one word character.
        // Notes fields must be 500 characters or less.

        var regex = new Regex("[a-zA-Z]", RegexOptions.Compiled, TimeSpan.FromMilliseconds(1000)); // Matches at least one alphbetic character anywhere in string.
        int maxLength = 500;
        string requiredErrorText = ViewResources.MoreDetailOnBusinessPlan.ResourceManager.GetString("required_error_message");
        string invalidErrorText = ViewResources.MoreDetailOnBusinessPlan.ResourceManager.GetString("invalid_error_message");
        string maxLengthErrorText = ViewResources.MoreDetailOnBusinessPlan.ResourceManager.GetString("maxlength_error_message");

        if (ShowInfrastructure)
        {
            if (string.IsNullOrEmpty(Infrastructure))
                yield return new ValidationResult(requiredErrorText, new[] { nameof(Infrastructure) });

            if (Infrastructure != null)
            {
                if (!regex.IsMatch(Infrastructure))
                    yield return new ValidationResult(invalidErrorText, new[] { nameof(Infrastructure) });

                if (Infrastructure.Length > maxLength)
                    yield return new ValidationResult(maxLengthErrorText, new[] { nameof(Infrastructure) });
            }
        }

        if (ShowPriceSupport)
        {
            if (string.IsNullOrEmpty(PriceSupport))
                yield return new ValidationResult(requiredErrorText, new[] { nameof(PriceSupport) });

            if (PriceSupport != null)
            {
                if (!regex.IsMatch(PriceSupport))
                    yield return new ValidationResult(invalidErrorText, new[] { nameof(PriceSupport) });

                if (PriceSupport.Length > maxLength)
                    yield return new ValidationResult(maxLengthErrorText, new[] { nameof(PriceSupport) });
            }
        }

        if (ShowBusinessCollections)
        {
            if (string.IsNullOrEmpty(BusinessCollections))
                yield return new ValidationResult(requiredErrorText, new[] { nameof(BusinessCollections) });

            if (BusinessCollections != null)
            {
                if (!regex.IsMatch(BusinessCollections))
                    yield return new ValidationResult(invalidErrorText, new[] { nameof(BusinessCollections) });

                if (BusinessCollections.Length > maxLength)
                    yield return new ValidationResult(maxLengthErrorText, new[] { nameof(BusinessCollections) });
            }
        }

        if (ShowCommunications)
        {
            if (string.IsNullOrEmpty(Communications))
                yield return new ValidationResult(requiredErrorText, new[] { nameof(Communications) });

            if (Communications != null)
            {
                if (!regex.IsMatch(Communications))
                    yield return new ValidationResult(invalidErrorText, new[] { nameof(Communications) });

                if (Communications.Length > maxLength)
                    yield return new ValidationResult(maxLengthErrorText, new[] { nameof(Communications) });
            }
        }

        if (ShowNewMarkets)
        {
            if (string.IsNullOrEmpty(NewMarkets))
                yield return new ValidationResult(requiredErrorText, new[] { nameof(NewMarkets) });

            if (NewMarkets != null)
            {
                if (!regex.IsMatch(NewMarkets))
                    yield return new ValidationResult(invalidErrorText, new[] { nameof(NewMarkets) });

                if (NewMarkets.Length > maxLength)
                    yield return new ValidationResult(maxLengthErrorText, new[] { nameof(NewMarkets) });
            }
        }

        if (ShowNewUses)
        {
            if (string.IsNullOrEmpty(NewUses))
                yield return new ValidationResult(requiredErrorText, new[] { nameof(NewUses) });

            if (NewUses != null)
            {
                if (!regex.IsMatch(NewUses))
                    yield return new ValidationResult(invalidErrorText, new[] { nameof(NewUses) });

                if (NewUses.Length > maxLength)
                    yield return new ValidationResult(maxLengthErrorText, new[] { nameof(NewUses) });
            }
        }

        if (ShowOther)
        {
            if (string.IsNullOrEmpty(Other))
                yield return new ValidationResult(requiredErrorText, new[] { nameof(Other) });

            if (Other != null)
            {
                if (!regex.IsMatch(Other))
                    yield return new ValidationResult(invalidErrorText, new[] { nameof(Other) });

                if (Other.Length > maxLength)
                    yield return new ValidationResult(maxLengthErrorText, new[] { nameof(Other) });
            }
        }
    }
}
