using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

/// <summary>
/// A generic model that can be reused for all permit screens that have a similar structure.
/// </summary>
[ExcludeFromCodeCoverage]
public class MaximumWeightSiteCanReprocessViewModel : MaterialPermitViewModel
{
    /// <summary>
    /// Sets the selected frequency option.
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(MaximumWeightSiteCanReprocess), ErrorMessageResourceName = "frequency_option_required_error_message")]
    public override MaterialFrequencyOptions? SelectedFrequency { get; set; }
}