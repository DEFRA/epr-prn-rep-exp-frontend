namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Represents a dto for updating the maximum weight for a material. 
/// </summary>
[ExcludeFromCodeCoverage]
public record UpdateMaximumWeightRequestDto
{
    /// <summary>
    /// The weight in tonnes that the processing site can handle for the material.
    /// </summary>
    public decimal WeightInTonnes { get; set; }

    /// <summary>
    /// The ID of the period that this relates to.
    /// </summary>
    public int PeriodId { get; set; }
}