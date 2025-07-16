using System.Globalization;
using Epr.Reprocessor.Exporter.UI.App.Resources.Enums;
using PermitType = Epr.Reprocessor.Exporter.UI.App.Domain.PermitType;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

/// <summary>
/// Represents a model for the check your answers for waste details page.
/// </summary>
public class CheckYourAnswersWasteDetailsViewModel
{
    /// <summary>
    /// Represents the data for the summary list that details the selected materials.
    /// </summary>
    public SummaryListModel Materials { get; } = new();

    /// <summary>
    /// Represents the data for the summary list that details the individual materials and the associated permit.
    /// </summary>
    public IList<SummaryListModel> MaterialPermits { get; } = [];

    /// <summary>
    /// Sets the packaging waste summary list details.
    /// </summary>
    /// <param name="packageWaste">The packaging waste to set.</param>
    public void SetPackagingWasteDetails(PackagingWaste packageWaste)
    {
        AddTopLevelMaterialsSelectedRow(packageWaste);
        AddMaterialSpecificSectionsWithPermitInformation(packageWaste);
    }

    #region Add rows

    private void AddMaterialSpecificSectionsWithPermitInformation(PackagingWaste packageWaste)
    {
        foreach (var material in packageWaste.SelectedMaterials)
        {
            AddMaterialSection(material);
        }
    }

    private void AddMaterialSection(RegistrationMaterial material)
    {
        var permitType = material.PermitType;
        var materialDisplayName = GetMaterialNameWithoutCode(material.Name);

        var materialSummaryList = new SummaryListModel
        {
            Heading = string.Format(CheckYourAnswersWasteDetails.summary_list_permit_sub_heading, materialDisplayName)
        };
            
        materialSummaryList.Rows.Add(permitType is PermitType.WasteExemption ? AddExemptionPermitRow(material) : AddPermitRow(material));
        materialSummaryList.Rows.AddRange(AddMaterialRows(material));

        MaterialPermits.Add(materialSummaryList);
    }

    private void AddTopLevelMaterialsSelectedRow(PackagingWaste packageWaste)
    {
        var selectedMaterials = string.Join(", ", packageWaste.SelectedMaterials.Select(o => GetMaterialNameWithoutCode(o.Name)));
        Materials.Rows.Add(CreateRow(
            CheckYourAnswersWasteDetails.summary_list_packaging_details_key,
            selectedMaterials,
            PagePaths.WastePermitExemptions,
            CheckYourAnswersWasteDetails.summary_list_packaging_details_change_link_aria_text));
    }

    private static SummaryListRowModel AddPermitRow(RegistrationMaterial material)
    {
        var value = material.PermitType is PermitType.WasteExemption ? string.Join(Environment.NewLine, material.Exemptions) : material.PermitNumber!;
        return CreateRow(
            material.PermitType!.GetDisplayName(),
            value,
            PagePaths.PermitForRecycleWaste,
            "the permit details"
        );
    }

    private static SummaryListRowModel AddExemptionPermitRow(RegistrationMaterial material) =>
        CreateRow(
            material.PermitType!.GetDisplayName(),
            string.Join(Environment.NewLine, material.Exemptions.Select(o => o.ReferenceNumber)),
            PagePaths.ExemptionReferences,
            "the exemption references"
        );

    private static List<SummaryListRowModel> AddMaterialRows(RegistrationMaterial material)
    {
        var rows = new List<SummaryListRowModel>();
        if (material.PermitType is not PermitType.WasteExemption)
        {
            rows.Add(AddPermitType(material));
            rows.Add(AddPermitPeriod(material));
        }

        rows.Add(AddMaximumWeightForSite(material));
        rows.Add(AddMaximumWeightPeriodForSite(material));

        return rows;
    }

    private static SummaryListRowModel AddMaximumWeightPeriodForSite(RegistrationMaterial material) =>
        CreateRow(
            CheckYourAnswersWasteDetails.summary_list_generic_period_key,
            PermitPeriodResource.ResourceManager.GetString(material.MaxCapableWeightPeriodDuration.ToString()!)!,
            PagePaths.MaximumWeightSiteCanReprocess,
            "the maximum weight period for the material"
        );

    private static SummaryListRowModel AddMaximumWeightForSite(RegistrationMaterial material)
    {
        var value = material.MaxCapableWeightInTonnes.HasValue ? material.MaxCapableWeightInTonnes.Value.ToString(CultureInfo.CurrentUICulture) : string.Empty;
        return CreateRow(
            CheckYourAnswersWasteDetails.summary_list_maximum_weight_site_tonnes_key, 
            value,
            PagePaths.MaximumWeightSiteCanReprocess, 
            "the maximum weight the site can reprocess for the material");
    }

    private static SummaryListRowModel AddPermitPeriod(RegistrationMaterial material) =>
        CreateRow(
            CheckYourAnswersWasteDetails.summary_list_generic_period_key,
            PermitPeriodResource.ResourceManager.GetString(material.PermitPeriod.ToString()!)!,
            ReprocessorExporterPermitTypeUrlProvider.Url(material.PermitType)!, 
            "the permit period for the material");

    private static SummaryListRowModel AddPermitType(RegistrationMaterial material)
    {
        var value = material.WeightInTonnes.HasValue ? material.WeightInTonnes.Value.ToString(CultureInfo.CurrentUICulture) : string.Empty;
        return CreateRow(
            CheckYourAnswersWasteDetails.summary_list_maximum_weight_permit_tonnes_key, 
            value,
            ReprocessorExporterPermitTypeUrlProvider.Url(material.PermitType)!, 
            "the permit weight for the material");
    }

    private static SummaryListRowModel CreateRow(string key, string value, string url, string ariaText) =>
        new()
        {
            Key = key,
            Value = value,
            ChangeLinkUrl = url,
            ChangeLinkHiddenAccessibleText = ariaText
        };

    private static string GetMaterialNameWithoutCode(Material material)
        => material.GetDisplayName().Substring(0, material.GetDisplayName().Length - 4).Trim();

    #endregion
}