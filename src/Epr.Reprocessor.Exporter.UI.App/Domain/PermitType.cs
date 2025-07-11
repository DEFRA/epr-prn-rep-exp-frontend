using System.ComponentModel.DataAnnotations;

namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Defines possible values for the permit type.
/// </summary>
public enum PermitType
{
    None = 0,

    [Display(ResourceType = typeof(Resources.Enums.PermitType), Name = "waste_exemption")]
    WasteExemption = 1,

    [Display(ResourceType = typeof(Resources.Enums.PermitType), Name = "ppc_permit")]
    PollutionPreventionAndControlPermit = 2,

    [Display(ResourceType = typeof(Resources.Enums.PermitType), Name = "waste_management_licence")]
    WasteManagementLicence = 3,

    [Display(ResourceType = typeof(Resources.Enums.PermitType), Name = "installation_permit")]
    InstallationPermit = 4,

    [Display(ResourceType = typeof(Resources.Enums.PermitType), Name = "environmental_permit_and_waste_management_licence")]
    EnvironmentalPermitOrWasteManagementLicence = 5,
}