namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Represents details of the license or permit that is associated with the material that is to be recycled.
/// </summary>
[ExcludeFromCodeCoverage]
public class LicenceOrPermit
{
    public decimal CapacityInTonnes { get; set; }

    public int PeriodId { get; set; }
}
