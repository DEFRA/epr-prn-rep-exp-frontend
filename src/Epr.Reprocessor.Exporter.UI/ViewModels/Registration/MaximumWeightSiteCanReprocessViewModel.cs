using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Validations.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

/// <summary>
/// A generic model that can be reused for all permit screens that have a similar structure.
/// </summary>
[ExcludeFromCodeCoverage]
public class MaximumWeightSiteCanReprocessViewModel : MaterialPermitViewModel
{
    /// <summary>
    /// The maximum weight limit for the permit
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(MaximumWeightSiteCanReprocess), ErrorMessageResourceName = "maximum_weight_required_error_message")]
    [TonnageValidation]
    public override string? MaximumWeight { get; set; }

    /// <summary>
    /// Sets the selected frequency option.
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(MaximumWeightSiteCanReprocess), ErrorMessageResourceName = "frequency_option_required_error_message")]
    public override MaterialFrequencyOptions? SelectedFrequency { get; set; }

    /// <summary>
    /// The type of permit.
    /// </summary>
    [BindNever]
    public PermitType? PermitTypeForMaterial { get; set; }

    /// <summary>
    /// Determines what the originating page was that got us to this maximum weight for reprocessing site page as we can come from any one of the permit screens.
    /// </summary>
    /// <param name="permitType">The type of permit being applied for, this is used to then determine the page the user came from.</param>
    /// <returns>The originating page.</returns>
    public string CalculateOriginatingPage(PermitType? permitType)
    {
        PermitTypeForMaterial = permitType;
        return PermitTypeForMaterial switch
        {
            PermitType.InstallationPermit => PagePaths.InstallationPermit,
            PermitType.WasteManagementLicence => PagePaths.WasteManagementLicense,
            PermitType.EnvironmentalPermitOrWasteManagementLicence => PagePaths.EnvironmentalPermitOrWasteManagementLicence,
            PermitType.PollutionPreventionAndControlPermit => PagePaths.PpcPermit,
            PermitType.WasteExemption => PagePaths.ExemptionReferences,
            PermitType.None => PagePaths.WastePermitExemptions,
            null => PagePaths.WastePermitExemptions,
            _ => throw new ArgumentOutOfRangeException(nameof(permitType))
        };
    }
}