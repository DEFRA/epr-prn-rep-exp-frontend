using Epr.Reprocessor.Exporter.UI.Resources.Views.Enums;
using System.Globalization;

namespace Epr.Reprocessor.Exporter.UI.ViewModels;

/// <summary>
/// Represents a model for the check your answers for waste details page.
/// </summary>
public class CheckYourAnswersWasteDetailsViewModel
{
    /// <summary>
    /// Represents the data for the summary list that details the selected materials.
    /// </summary>
    public SummaryListModel PackagingWasteDetailsSummaryList { get; } = new();

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
        var selectedMaterials = string.Join(", ", packageWaste.SelectedMaterials.Select(o => GetMaterialNameWithoutCode(o.Name)));
        PackagingWasteDetailsSummaryList.Rows.Add(new()
        {
            Key = CheckYourAnswersWasteDetails.summary_list_packaging_details_key,
            Value = selectedMaterials,
            ChangeLinkUrl = PagePaths.WastePermitExemptions,
            ChangeLinkHiddenAccessibleText = CheckYourAnswersWasteDetails.summary_list_packaging_details_change_link_aria_text
        });

        foreach (var material in packageWaste.SelectedMaterials)
        {
            var permitType = material.PermitType;
            var materialDisplayName = GetMaterialNameWithoutCode(material.Name);

            var materialSummaryList = new SummaryListModel
            {
                Heading = string.Format(CheckYourAnswersWasteDetails.summary_list_permit_sub_heading, materialDisplayName)
            };
            
            materialSummaryList.Rows.Add(permitType is PermitType.WasteExemption
                ? AddExemptionPermitRow(material)
                : AddPermitRow(material));

            materialSummaryList.Rows.AddRange(AddMaterialDetailRows(material));

            MaterialPermits.Add(materialSummaryList);
        }
    }

    private static SummaryListRowModel AddPermitRow(RegistrationMaterial material) =>
        new()
        {
            Key = material.PermitType!.GetDisplayName(),
            Value = material.PermitType is PermitType.WasteExemption ? string.Join(Environment.NewLine, material.Exemptions) : material.PermitNumber!,
            ChangeLinkUrl = PagePaths.PermitForRecycleWaste
        };

    private static SummaryListRowModel AddExemptionPermitRow(RegistrationMaterial material) =>
        new()
        {
            Key = material.PermitType!.GetDisplayName(),
            Value = string.Join(Environment.NewLine, material.Exemptions.Select(o => o.ReferenceNumber))
        };

    private static List<SummaryListRowModel> AddMaterialDetailRows(RegistrationMaterial material) =>
    [
        new()
        {
            Key = CheckYourAnswersWasteDetails.summary_list_maximum_weight_permit_tonnes_key,
            Value = material.WeightInTonnes.HasValue ? material.WeightInTonnes.Value.ToString(CultureInfo.CurrentUICulture) : string.Empty
        },


        new()
        {
            Key = CheckYourAnswersWasteDetails.summary_list_generic_period_key,
            Value = MaterialFrequencyOptionsResource.ResourceManager.GetString(material.PermitPeriod.ToString()!)!
        },


        new()
        {
            Key = CheckYourAnswersWasteDetails.summary_list_maximum_weight_site_tonnes_key,
            Value = material.MaxCapableWeightInTonnes.HasValue ? material.MaxCapableWeightInTonnes.Value.ToString(CultureInfo.CurrentUICulture) : string.Empty
        },


        new()
        {
            Key = CheckYourAnswersWasteDetails.summary_list_generic_period_key,
            Value = MaterialFrequencyOptionsResource.ResourceManager.GetString(material.MaxCapableWeightPeriodDuration.ToString())!
        }
    ];

    private static string GetMaterialNameWithoutCode(Material material)
        => material.GetDisplayName().Substring(0, material.GetDisplayName().Length - 4).Trim();
}