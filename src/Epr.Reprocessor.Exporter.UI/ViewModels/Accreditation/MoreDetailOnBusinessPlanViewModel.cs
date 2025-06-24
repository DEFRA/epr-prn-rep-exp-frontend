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
        foreach (var validationResult in ValidateField(ShowInfrastructure, Infrastructure, nameof(Infrastructure)))
            yield return validationResult;

        foreach (var validationResult in ValidateField(ShowPriceSupport, PriceSupport, nameof(PriceSupport)))
            yield return validationResult;

        foreach (var validationResult in ValidateField(ShowBusinessCollections, BusinessCollections, nameof(BusinessCollections)))
            yield return validationResult;

        foreach (var validationResult in ValidateField(ShowCommunications, Communications, nameof(Communications)))
            yield return validationResult;

        foreach (var validationResult in ValidateField(ShowNewMarkets, NewMarkets, nameof(NewMarkets)))
            yield return validationResult;

        foreach (var validationResult in ValidateField(ShowNewUses, NewUses, nameof(NewUses)))
            yield return validationResult;

        foreach (var validationResult in ValidateField(ShowOther, Other, nameof(Other)))
            yield return validationResult;
    }

    private Regex regex = new Regex("[a-zA-Z]", RegexOptions.Compiled);
    private int maxLength = 500;

    private IEnumerable<ValidationResult> ValidateField(
        bool showField,
        string? fieldValue,
        string fieldName)
    {
        if (showField)
        {
            if (string.IsNullOrEmpty(fieldValue))
            {
                yield return new ValidationResult(String.Format(ViewResources.MoreDetailOnBusinessPlan.ResourceManager.GetString("required_error_message"), Subject), new[] { fieldName });
            }
            else
            {
                if (fieldValue.Length > maxLength)
                {
                    yield return new ValidationResult(ViewResources.MoreDetailOnBusinessPlan.ResourceManager.GetString("maxlength_error_message"), new[] { fieldName });
                }
                else if (!regex.IsMatch(fieldValue))
                {
                    yield return new ValidationResult(ViewResources.MoreDetailOnBusinessPlan.ResourceManager.GetString("invalid_error_message"), new[] { fieldName });
                }                
            }
        }
    }
}
