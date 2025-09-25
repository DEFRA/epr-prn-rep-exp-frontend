using System.ComponentModel.DataAnnotations;
using Epr.Reprocessor.Exporter.UI.App.Helpers;

namespace Epr.Reprocessor.Exporter.UI.ViewModels.Registration;

/// <summary>
/// Defines a view model for the select permit type page.
/// </summary>
[ExcludeFromCodeCoverage]
public class SelectAuthorisationTypeViewModel
{
    /// <summary>
    /// Collection of permit types that can be selected.
    /// </summary>
    public List<AuthorisationTypes> AuthorisationTypes { get; set; } = null!;

    /// <summary>
    /// The ID of the selected permit. 
    /// </summary>
    [Required(ErrorMessageResourceName = "error_message_no_selection", ErrorMessageResourceType = typeof(SelectAuthorisationType))]
    public int? SelectedAuthorisation { get; set; }

    /// <summary>
    /// The associated nation code, used to determine what permits to display as only specific types are applicable for different nations.
    /// </summary>
    public string? NationCode { get; set; }

    /// <summary>
    /// The name of the currently selected material.
    /// </summary>
    public Material SelectedMaterial { get; set; } = Material.None;
}

/// <summary>
/// Defines an individual permit that can be selected.
/// </summary>
[ExcludeFromCodeCoverage]
public class AuthorisationTypes
{
    /// <summary>
    /// The internal ID of the permit.
    /// </summary>
    public int? Id { get; set; }
    
    /// <summary>
    /// The name of the permit.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The label text for the radio button that is displayed for the permit.
    /// </summary>
    public string? Label { get; set; }
    
    /// <summary>
    /// The associated permit number for the selected permit type, applicable to all permits except for <see cref="PermitType.WasteExemption"/>.
    /// </summary>
    public string? SelectedAuthorisationText { get; set; }

    /// <summary>
    /// The nation code category that the permit falls within.
    /// </summary>
    public List<string>? NationCodeCategory { get; set; }
}