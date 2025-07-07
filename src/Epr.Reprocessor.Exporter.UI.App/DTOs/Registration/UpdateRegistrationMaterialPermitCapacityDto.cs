namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Dto to update permit-related information for a registration material permit capacity.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateRegistrationMaterialPermitCapacityDto
{
    /// <summary>
    /// Gets or sets the ID of the permit type.
    /// </summary>
    public int PermitTypeId { get; set; }

    /// <summary>
    /// Gets or sets the capacity in tones.
    /// </summary>
    public decimal? CapacityInTonnes { get; set; }

    /// <summary>
    /// Gets or sets the period id.
    /// </summary>
    public int? PeriodId { get; set; }
}