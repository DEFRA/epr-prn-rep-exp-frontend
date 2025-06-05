namespace Epr.Reprocessor.Exporter.UI.Domain;

/// <summary>
/// Represents details of the material that is to be recycled as part of the packaging waste.
/// </summary>
[ExcludeFromCodeCoverage]
public class Material
{
    /// <summary>
    /// The name of the material i.e. Steel, Wood etc.
    /// </summary>
    public string Name { get; set; } = null!;

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
    public Exemption Exemption { get; set; } = new Exemption();

    /// <summary>
    /// Flag that determines if the material has been applied for in the registration application.
    /// </summary>
    public bool Applied { get; set; }

    public Material SetExemptions(Exemption exemption)
    {
        Exemption = exemption;
        return this;
    }
}