using System.ComponentModel.DataAnnotations;
using ViewResources = Epr.Reprocessor.Exporter.UI.Resources.Views.Accreditation;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Accreditation;

[ExcludeFromCodeCoverage]
public class MoreDetailOnBusinessPlanViewModel : IValidatableObject
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

    private const int MaxLength = 500;

    private IEnumerable<ValidationResult> ValidateField(
        bool showField,
        string? fieldValue,
        string fieldName)
    {
        if (!showField)
            yield break;

        if (string.IsNullOrWhiteSpace(fieldValue))
        {
            yield return new ValidationResult(
                string.Format(ViewResources.MoreDetailOnBusinessPlan.ResourceManager.GetString("required_error_message"), Subject),
                new[] { fieldName }
            );
            yield break;
        }

        // Safely handle the input - decode any HTML entities first
        var decodedValue = System.Net.WebUtility.HtmlDecode(fieldValue);

        // Check normalized length as textarea counts new lines as single char
        var normalizedValue = decodedValue.Replace("\r\n", "\n");
        if (normalizedValue.Length > MaxLength)
        {
            yield return new ValidationResult(
                ViewResources.MoreDetailOnBusinessPlan.ResourceManager.GetString("maxlength_error_message"),
                new[] { fieldName }
            );
            yield break;
        }

        // For description fields, allow any printable characters including symbols
        // Just check that it's not only whitespace/control characters
        if (!HasMeaningfulContent(decodedValue))
        {
            yield return new ValidationResult(
                ViewResources.MoreDetailOnBusinessPlan.ResourceManager.GetString("invalid_error_message"),
                new[] { fieldName }
            );
        }
    }

    private static bool HasMeaningfulContent(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        // Check for actual letters (a-z, A-Z) in the original input
        // This prevents HTML entities like &amp; from being counted as letters
        return value.Any(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
    }

    // Property setters to safely encode dangerous characters
    private string? _infrastructure;
    public string? Infrastructure
    {
        get => _infrastructure = DesanitizeInput(_infrastructure);
        set => _infrastructure = SanitizeInput(value);
    }

    private string? _priceSupport;
    public string? PriceSupport
    {
        get => _priceSupport = DesanitizeInput(_priceSupport);
        set => _priceSupport = SanitizeInput(value);
    }

    private string? _businessCollections;
    public string? BusinessCollections
    {
        get => _businessCollections = DesanitizeInput(_businessCollections);
        set => _businessCollections = SanitizeInput(value);
    }

    private string? _communications;
    public string? Communications
    {
        get => _communications = DesanitizeInput(_communications);
        set => _communications = SanitizeInput(value);
    }

    private string? _newMarkets;
    public string? NewMarkets
    {
        get => _newMarkets = DesanitizeInput(_newMarkets);
        set => _newMarkets = SanitizeInput(value);
    }

    private string? _newUses;
    public string? NewUses
    {
        get => _newUses = DesanitizeInput(_newUses);
        set => _newUses = SanitizeInput(value);
    }

    private string? _other;
    public string? Other
    {
        get => _other = DesanitizeInput(_other);
        set => _other = SanitizeInput(value);
    }

    private static string? SanitizeInput(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Encode dangerous HTML characters but preserve the content
        return System.Net.WebUtility.HtmlEncode(input);
    }

    private static string? DesanitizeInput(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Decode back to show correct HTML characters
        return System.Net.WebUtility.HtmlDecode(input);
    }
}