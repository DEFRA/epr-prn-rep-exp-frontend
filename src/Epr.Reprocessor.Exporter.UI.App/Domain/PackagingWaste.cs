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
    /// Collection of all available materials that can be selected.
    /// </summary>
    public List<MaterialLookupDto> AllMaterials { get; set; } = new();

    /// <summary>
    /// Collection of materials that have been selected.
    /// </summary>
    public List<Material> SelectedMaterials { get; set; } = new();

    /// <summary>
    /// Selected Authorization Id
    /// </summary>
    public int? SelectedAuthorisation { get; set; }

    /// <summary>
    /// Selected Authorization Text
    /// </summary>
    public string? SelectedAuthorisationText { get; set; }

    /// <summary>
    /// Waste Management Licence
    /// </summary>
    public LicenceOrPermit WasteManagementLicence { get; set; }

    /// <summary>
    /// Installation Permit
    /// </summary>
    public LicenceOrPermit InstallationPermit { get; set; }
    

    /// <summary>
    /// Determines the next material that is eligible to be applied for in the registration application based on the next material in the list in alphabetical order that has not been applied for yet.
    /// </summary>
    public Material? CurrentMaterialApplyingFor => SelectedMaterials.OrderBy(o => o.Name).FirstOrDefault(o => !o.Applied);

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
    public PackagingWaste SetFromExisting(IEnumerable<RegistrationMaterialDto> materials)
    {
        var proposedMaterials = materials.ToList();
        if (proposedMaterials.Count == 0)
        {
            // Set an empty list if theres no existing materials, caters for when a material is deleted, and we need to set the session accordingly.
            SelectedMaterials = new List<Material>();
        }
        else
        {
            SelectedMaterials = proposedMaterials.Select(o => new Material
            {
                Id = o.Id,
                Applied = o.IsMaterialBeingAppliedFor.GetValueOrDefault(),
                Name = o.MaterialLookup.Name,
                Exemptions = o.ExemptionReferences.Select(x => new Exemption
                    {
                        ReferenceNumber = x.ReferenceNumber
                    })
                    .ToList()

            }).ToList();
        }

        return this;
    }

    /// <summary>
    /// Handles when a new registration material has been created and sets into session accordingly.
    /// </summary>
    /// <param name="registrationMaterial">Collection of materials to set.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste RegistrationMaterialCreated(RegistrationMaterialDto registrationMaterial)
    {
        if (SelectedMaterials.Exists(o => o.Id == registrationMaterial.Id))
        {
            return this;
        }

        SelectedMaterials.Add(new Material
        {
            Id = registrationMaterial.Id,
            Name = registrationMaterial.MaterialLookup.Name,
            Applied = registrationMaterial.IsMaterialBeingAppliedFor.GetValueOrDefault(),
            Exemptions = registrationMaterial.ExemptionReferences.Select(o => new Exemption
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
    public PackagingWaste SetMaterialAsApplied(MaterialItem material)
    {
        SelectedMaterials.Single(o => o.Name == material).Applied = true;

        return this;
    }

    /// <summary>
    /// Sets the waste management licence
    /// </summary>
    /// <param name="licenceOrPermit">The licence</param>
    /// <returns></returns>
    public PackagingWaste SetWasteManagementLicence(LicenceOrPermit licenceOrPermit)
    {
        WasteManagementLicence = licenceOrPermit; 
        return this;
    }

    /// <summary>
    /// Sets the installation permit
    /// </summary>
    /// <param name="licenceOrPermit">The permit</param>
    /// <returns></returns>
    public PackagingWaste SetInstallationPermit(LicenceOrPermit licenceOrPermit)
    {
        InstallationPermit = licenceOrPermit;
        return this;
    }
}