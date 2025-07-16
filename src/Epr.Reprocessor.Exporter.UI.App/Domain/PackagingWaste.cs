using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Domain;

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
    public Guid? RegistrationMaterialId => CurrentMaterialApplyingFor?.Id;
    
    /// <summary>
    /// Collection of materials that have been selected.
    /// </summary>
    public List<RegistrationMaterial> SelectedMaterials { get; set; } = new();

    /// <summary>
    /// Determines the next material that is eligible to be applied for in the registration application based on the next material in the list in alphabetical order that has not been applied for yet.
    /// </summary>
    public RegistrationMaterial? CurrentMaterialApplyingFor => SelectedMaterials.OrderBy(o => o.Name.ToString()).FirstOrDefault(o => !o.Applied);

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
    /// <param name="permitType">The Permit Type.</param>
    /// <param name="permitNumber">The Permit Number.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetSelectedAuthorisation(PermitType? permitType, string? permitNumber)
    {
        // If the permit type has changed, we want to 'reset' the permit tonnage and frequency values.
        if (CurrentMaterialApplyingFor!.PermitType != permitType)
        {
            CurrentMaterialApplyingFor!.WeightInTonnes = 0;
            CurrentMaterialApplyingFor!.PermitPeriod = null;
        }

        CurrentMaterialApplyingFor!.PermitType = permitType;
        CurrentMaterialApplyingFor!.PermitNumber = permitNumber;

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
    /// Sets the Environmental Permit or Waste Management Licence.
    /// </summary>
    /// <param name="weightInTonnes">The weight in tonnes related to the permit.</param>
    /// <param name="periodId">The ID of the period within which the permit applies.</param>
    /// <returns>This instance.</returns>
    public PackagingWaste SetEnvironmentalPermitOrWasteManagementLicence(decimal weightInTonnes, int periodId)
    {
        CurrentMaterialApplyingFor!.SetPermitWeightDetails(PermitType.EnvironmentalPermitOrWasteManagementLicence, weightInTonnes, periodId);

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

    #region Validation

    /// <summary>
    /// Validates if all the packaging waste details have been provided meaning we can show the check your answers page.
    /// </summary>
    /// <returns><c>True</c> if everything has been provided, <c>False</c> otherwise.</returns>
    public bool ValidateForCheckYourAnswers() =>
        SelectedMaterials.Count > 0 && SelectedMaterials.TrueForAll(ValidateMaterial);

    private static bool ValidateMaterial(RegistrationMaterial material) =>
        ValidatePermitDetails(material) &&
        ValidateMaximumWeightReprocessingSiteCanHandle(material);

    private static bool ValidateMaximumWeightReprocessingSiteCanHandle(RegistrationMaterial material) =>
        material.MaxCapableWeightPeriodDuration is not PeriodDuration.None &&
        material.MaxCapableWeightInTonnes is not null or 0;

    private static bool ValidatePermitDetails(RegistrationMaterial material)
    {
        if (material.PermitType is PermitType.WasteExemption)
        {
            if (material.Exemptions.Count is 0)
            {
                return false;
            }
        }
        else
        {
            if (!ValidatePermitWeightAndFrequency(material))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ValidatePermitWeightAndFrequency(RegistrationMaterial material)
    {
        if (material.PermitType is PermitType.WasteExemption)
        {
            return true;
        }

        return !string.IsNullOrEmpty(material.PermitNumber) &&
               material.WeightInTonnes is not 0 &&
               material.PermitPeriod is not (null or 0);
    }

    #endregion
}