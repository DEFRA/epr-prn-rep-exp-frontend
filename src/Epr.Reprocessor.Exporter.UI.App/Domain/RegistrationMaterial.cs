using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Represents details of the material that is to be recycled as part of the packaging waste.
/// </summary>
[ExcludeFromCodeCoverage]
public class RegistrationMaterial
{
    /// <summary>
    /// The unique identifier for the material.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the material i.e. Steel, Wood etc.
    /// </summary>
    [JsonConverter(typeof(MaterialItemConverter))]
    public Material Name { get; set; }

    /// <summary>
    /// The type of permit that this material has applied for.
    /// </summary>
    public PermitType? PermitType { get; set; }

    /// <summary>
    /// The period for the permit type that has been applied for i.e. Per year.
    /// </summary>
    public PermitPeriod? PermitPeriod { get; set; }

    /// <summary>
    /// The current status for the material registration.
    /// </summary>
    public MaterialStatus Status { get; set; }

    /// <summary>
    /// The permit number for the permit being applied for.
    /// </summary>
    public string? PermitNumber { get; set; }

    /// <summary>
    /// The weight in tonnes that the reprocessing site will recycle for the <see cref="Name"/> of the material that is being applied for.
    /// </summary>
    /// <remarks>Only applies if <see cref="PermitType"/> is anything but <see cref="Domain.PermitType.WasteExemption"/>.</remarks>
    public decimal WeightInTonnes { get; set; }
    
    /// <summary>
    /// Any exemptions associated with the material that is to be recycled.
    /// </summary>
    public IList<Exemption> Exemptions { get; set; } = new List<Exemption>();

    /// <summary>
    /// Flag that determines if the material has been applied for in the registration application.
    /// </summary>
    public bool Applied { get; set; }

    /// <summary>
    /// Sets the exemptions for this registration material, only applies if <see cref="PermitType"/> is <see cref="Domain.PermitType.WasteExemption"/>.
    /// </summary>
    /// <param name="exemptions">The collection of exemptions to be set.</param>
    /// <returns>This instance.</returns>
    public RegistrationMaterial SetExemptions(IList<Exemption> exemptions)
    {
        Exemptions = exemptions;
        PermitType = Domain.PermitType.WasteExemption;
        
        return this;
    }

    /// <summary>
    /// Sets the details of the corresponding permit for this registration material.
    /// </summary>
    /// <param name="permitType">The type of permit being set.</param>
    /// <param name="weightInTonnes">The weight in tonnes for the material for the permit.</param>
    /// <param name="periodId">The ID of the period within which this permit applies to.</param>
    /// <returns>This instance.</returns>
    public RegistrationMaterial SetPermitWeightDetails(PermitType permitType, decimal weightInTonnes, int periodId)
    {
        PermitType = permitType; 
        WeightInTonnes = weightInTonnes;
        PermitPeriod = (PermitPeriod)periodId;

        return this;
    }
}