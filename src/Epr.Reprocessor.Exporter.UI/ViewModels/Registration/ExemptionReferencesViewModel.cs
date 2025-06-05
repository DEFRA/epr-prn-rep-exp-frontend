using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

/// <summary>
/// The model that handles the data for the Exemption References View.
/// </summary>
[ExcludeFromCodeCoverage]
public class ExemptionReferencesViewModel : IValidatableObject
{
    /// <summary>
    /// The name of the material that the exemption references are associated with.
    /// </summary>
    public string? MaterialName { get; set; } = "Plastic";

    /// <summary>
    /// The first exemption reference for the permit.
    /// </summary>
    [RegularExpression(@"^[a-zA-Z0-9/]*$", ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "exemption_reference_invalid_format_error_message")]
    [StringLength(20, ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "exemption_reference_length_error_message")]
    public string? ExemptionReferences1 { get; set; }

    /// <summary>
    /// The second exemption reference for the permit, this is optional.
    /// </summary>
    [RegularExpression(@"^[a-zA-Z0-9/]*$", ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "exemption_reference_invalid_format_error_message")]
    [StringLength(20, ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "exemption_reference_length_error_message")]
    public string? ExemptionReferences2 { get; set; }

    /// <summary>
    /// The third exemption reference for the permit, this is optional.
    /// </summary>
    [RegularExpression(@"^[a-zA-Z0-9/]*$", ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "exemption_reference_invalid_format_error_message")]
    [StringLength(20, ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "exemption_reference_length_error_message")]
    public string? ExemptionReferences3 { get; set; }

    /// <summary>
    /// The fourth exemption reference for the permit, this is optional.
    /// </summary>
    [RegularExpression(@"^[a-zA-Z0-9/]*$", ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "exemption_reference_invalid_format_error_message")]
    [StringLength(20, ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "exemption_reference_length_error_message")]
    public string? ExemptionReferences4 { get; set; }

    /// <summary>
    /// The fifth exemption reference for the permit, this is optional.
    /// </summary>
    [RegularExpression(@"^[a-zA-Z0-9/]*$", ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "exemption_reference_invalid_format_error_message")]
    [StringLength(20, ErrorMessageResourceType = typeof(ExemptionReferences), ErrorMessageResourceName = "exemption_reference_length_error_message")]
    public string? ExemptionReferences5 { get; set; }

    /// <summary>
    /// Validate the view model.
    /// </summary>
    /// <param name="validationContext">Contextual information around the validation.</param>
    /// <returns>Collection of validation results.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Ensure not all entries have been left empty.
        if (string.IsNullOrWhiteSpace(ExemptionReferences1) &&
            string.IsNullOrWhiteSpace(ExemptionReferences2) && 
            string.IsNullOrWhiteSpace(ExemptionReferences3) && 
            string.IsNullOrWhiteSpace(ExemptionReferences4) && 
            string.IsNullOrWhiteSpace(ExemptionReferences5))
        {
            yield return new ValidationResult(ExemptionReferences.exemption_reference_required_error_message, new List<string>{nameof(ExemptionReferences1)});
        }

        // Checking if the entered reference number already exists in any of the other reference number properties./
        // Transform into a ExemptionValues object as not only do we want to find the duplicates. we also want to know which field is the duplicate.
        // So that we can assign the error message accordingly.
        var allReferenceNumbers = new List<ExemptionValues>
        {
            new() { Value = ExemptionReferences1!, NameOfField = nameof(ExemptionReferences1) },
            new() { Value = ExemptionReferences2!, NameOfField = nameof(ExemptionReferences2) },
            new() { Value = ExemptionReferences3!, NameOfField = nameof(ExemptionReferences3) },
            new() { Value = ExemptionReferences4!, NameOfField = nameof(ExemptionReferences4) },
            new() { Value = ExemptionReferences5!, NameOfField = nameof(ExemptionReferences5) }
        };

        // Remove empty entries from the list
        allReferenceNumbers.RemoveAll(x => string.IsNullOrEmpty(x.Value));

        // Get all duplicates.
        var duplicates = allReferenceNumbers
            .GroupBy(x => x.Value)
            .Where(g => g.Count() > 1)
            .Select(g => g)
            .ToList();

        // If there are duplicates then loop through each grouping.
        // Each grouping has a Key which is the value that is duplicated and then the values are the ExemptionValues objects which shows the field name.
        // We want to get the field name of the last item in the list as this will be the actual field that contains the duplicate value.
        // For example, if ExemptionReferences1 and ExemptionReferences2 are both "123" then we want to show the error message on ExemptionReferences2 as this is the point in the list where the duplicate was found.
        if (duplicates.Count > 0)
        {
            foreach (var duplicate in duplicates)
            {
                // For each duplicate, the first two entries in duplicate.Select(x => x) correlates to one field, so we check
                // if we have more than 2 for the field meaning there is at least three fields duplicated, in this scenario we skip the first field and then show 
                // an error on the second and third field as these are where the value has been duplicated.
                // For example, ExemptionReferences1 = "123", ExemptionReferences2 = "123", ExemptionReferences3 = "123" then we want to show the error on ExemptionReferences2 and ExemptionReferences3,
                var skipAmount = duplicate.Select(x => x).Count() >= 2 ? 1 : 0;
                var memberNames = duplicate.Skip(skipAmount).Select(x => x.NameOfField).ToList();

                yield return new ValidationResult(ExemptionReferences.exemption_reference_duplicate_error_message, memberNames);
            }
        }
    }
}

[ExcludeFromCodeCoverage]
public record ExemptionValues
{
    public string Value { get; set; } = null!;

    public string NameOfField { get; set; } = null!;
}