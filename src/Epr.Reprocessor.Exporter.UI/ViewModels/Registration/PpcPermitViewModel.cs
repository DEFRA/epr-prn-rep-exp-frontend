using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Epr.Reprocessor.Exporter.UI.Enums;
using Epr.Reprocessor.Exporter.UI.Resources.Views.Registration;
using Epr.Reprocessor.Exporter.UI.Validations.Attributes;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

/// <summary>
/// The model that handles the data for the PPC Permit View.
/// </summary>
[ExcludeFromCodeCoverage]
public class PpcPermitViewModel
{
    /// <summary>
    /// Defines the available options for the frequency radio buttons.
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(PpcPermit), ErrorMessageResourceName = "ppc_permit_frequency_option_required_error_message")]
    public PpcPermitFrequencyOptions? FrequencyOptions { get; set; }

    /// <summary>
    /// The maximum weight limit for the permit
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(PpcPermit), ErrorMessageResourceName = "ppc_permit_maximum_weight_required_error_message")]
    [TonnageValidation]
    public int? MaximumWeight { get; set; }
}