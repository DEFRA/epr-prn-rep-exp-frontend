using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared.Partials;
using Epr.Reprocessor.Exporter.UI.Validations.Attributes;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

/// <summary>
/// A generic model that can be reused for all permit screens that have a similar structure.
/// </summary>
[ExcludeFromCodeCoverage]
public class MaterialPermitViewModel
{
    /// <summary>
    /// The material that this permit applies to.
    /// <remarks>Hardcoded for now till the journey is complete.</remarks>
    /// </summary>
    public string? Material { get; set; } = "steel";

    /// <summary>
    /// Sets the selected frequency option.
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(MaterialPermitInput), ErrorMessageResourceName = "frequency_option_required_error_message")]
    public virtual PermitPeriod? SelectedFrequency { get; set; }

    /// <summary>
    /// The maximum weight limit for the permit
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(MaterialPermitInput), ErrorMessageResourceName = "maximum_weight_required_error_message")]
    [TonnageValidation]
    public virtual string? MaximumWeight { get; set; }

    /// <summary>
    /// The type of material permit that the weight is being set for.
    /// </summary>
    public MaterialType MaterialType { get; set; }
}