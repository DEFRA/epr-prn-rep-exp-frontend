using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Validations.Attributes;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

/// <summary>
/// A generic model that can be reused for all permit screens that have a similar structure.
/// </summary>
public record MaximumWeightSiteCanReprocessViewModel : MaterialPermitViewModel
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
    public override PermitPeriod? SelectedFrequency { get; set; }
}