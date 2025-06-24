using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Represents details of the material that is to be recycled as part of the packaging waste.
/// </summary>
[ExcludeFromCodeCoverage]
public class Material
{
    /// <summary>
    /// The unique identifier for the material.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the material i.e. Steel, Wood etc.
    /// </summary>
    public MaterialItem Name { get; set; }

    /// <summary>
    /// Any permits associated with the material that is to be recycled.
    /// </summary>
    public IList<Permit> Permits { get; set; } = new List<Permit>();

    /// <summary>
    /// Any licences associated with the material that is to be recycled.
    /// </summary>
    public IList<License> Licences { get; set; } = new List<License>();

    /// <summary>
    /// Any exemptions associated with the material that is to be recycled.
    /// </summary>
    public IList<Exemption> Exemptions { get; set; } = new List<Exemption>();

    /// <summary>
    /// Flag that determines if the material has been applied for in the registration application.
    /// </summary>
    public bool Applied { get; set; }

    public Material SetExemptions(List<Exemption> exemptions)
    {
        Exemptions = exemptions;
        
        return this;
    }
}