using Epr.Reprocessor.Exporter.UI.App.Domain;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.Domain;

/// <summary>
/// Represents details of the materials that form part of the packaging waste that is to be recycled.
/// <remarks>2nd Section.</remarks>
/// </summary>
[ExcludeFromCodeCoverage]
public class PackagingWaste
{
    /// <summary>
    /// Registration Material External ID
    /// </summary>
    public Guid? RegistrationMaterialId {  get; set; }
    
    /// <summary>
    /// Collection of materials that have been selected.
    /// </summary>
    public List<RegistrationMaterial> SelectedMaterials { get; set; } = new();

    /// <summary>
    /// Selected Authorization Id
    /// </summary>
    public int? SelectedAuthorisation { get; set; }

    /// <summary>
    /// Selected Authorization Text
    /// </summary>
    public string? SelectedAuthorisationText { get; set; }

    /// <summary>
    /// Determines the next material that is eligible to be applied for in the registration application based on the next material in the list in alphabetical order that has not been applied for yet.
    /// </summary>
    public RegistrationMaterial? CurrentMaterialApplyingFor => SelectedMaterials.OrderBy(o => o.Name).FirstOrDefault(o => !o.Applied);

    /// <summary>
    /// Sets the registration material ID for the packaging waste.
    /// </summary>
    /// <param name="registrationMaterialId">The registration material id.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetRegistrationMaterialId(Guid? registrationMaterialId)
    {
        RegistrationMaterialId = registrationMaterialId;

        return this;
    }

    /// <summary>
    /// Sets from the existing registration materials into the session.
    /// </summary>
    /// <param name="materials">Collection of materials to set.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetFromExisting(IEnumerable<RegistrationMaterial> materials)
    {
        var proposedMaterials = materials.ToList();
        if (proposedMaterials.Count is 0)
        {
            // Set an empty list if there is no existing materials, caters for when a material is deleted, and we need to set the session accordingly.
            SelectedMaterials = new List<RegistrationMaterial>();
        }
        else
        {
            SelectedMaterials = proposedMaterials;
        }

        return this;
    }

    /// <summary>
    /// Handles when a new registration material has been created and sets into session accordingly.
    /// </summary>
    /// <param name="registrationMaterial">Collection of materials to set.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste RegistrationMaterialCreated(RegistrationMaterial registrationMaterial)
    {
        if (SelectedMaterials.Exists(o => o.Id == registrationMaterial.Id))
        {
            return this;
        }

        SelectedMaterials.Add(new RegistrationMaterial
        {
            Id = registrationMaterial.Id,
            Name = registrationMaterial.Name,
            Applied = registrationMaterial.Applied,
            Exemptions = registrationMaterial.Exemptions.Select(o => new Exemption
            {
                ReferenceNumber = o.ReferenceNumber,
            }).ToList()
        });

        return this;
    }

    /// <summary>
    /// Sets the selected permit type and permit number.
    /// </summary>
    /// <param name="selectedAuthorisation">Selected authorization Id.</param>
    /// <param name="selectedAuthorisationText">Selected authorization Text.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetSelectedAuthorisation(int? selectedAuthorisation, string? selectedAuthorisationText)
    {
        SelectedAuthorisation = selectedAuthorisation;
        SelectedAuthorisationText = selectedAuthorisationText;

        return this;
    }

    /// <summary>
    /// Sets the specified material to applied.
    /// </summary>
    /// <param name="material">The material to set to applied.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetMaterialAsApplied(Material material)
    {
        SelectedMaterials.Single(o => o.Name == material).Applied = true;

        return this;
    }
    
    /// <summary>
    /// Sets the waste management licence
    /// </summary>
    /// <param name="weightInTonnes">The weight in tonnes related to the permit.</param>
    /// <param name="periodId">The ID of the period within which the permit applies.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetWasteManagementLicence(decimal weightInTonnes, int periodId)
    {
        CurrentMaterialApplyingFor!.SetPermitWeightDetails(PermitType.WasteManagementLicence, weightInTonnes, periodId);

        return this;
    }

    /// <summary>
    /// Sets the installation permit
    /// </summary>
    /// <param name="weightInTonnes">The weight in tonnes related to the permit.</param>
    /// <param name="periodId">The ID of the period within which the permit applies.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetInstallationPermit(decimal weightInTonnes, int periodId)
    {
        CurrentMaterialApplyingFor!.SetPermitWeightDetails(PermitType.InstallationPermit, weightInTonnes, periodId);

        return this;
    }

    /// <summary>
    /// Sets the PPC permita
    /// </summary>
    /// <param name="weightInTonnes">The weight in tonnes related to the permit.</param>
    /// <param name="periodId">The ID of the period within which the permit applies.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetPPCPermit(decimal weightInTonnes, int periodId)
    {
        CurrentMaterialApplyingFor!.SetPermitWeightDetails(PermitType.PollutionPreventionAndControlPermit, weightInTonnes, periodId);

        return this;
    }
}