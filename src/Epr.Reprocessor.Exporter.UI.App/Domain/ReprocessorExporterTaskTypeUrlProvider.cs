using Epr.Reprocessor.Exporter.UI.App.DTOs.TaskList;

namespace Epr.Reprocessor.Exporter.UI.App.Domain;

/// <summary>
/// Provides a means to retrieve the Url for a task.
/// </summary>
public static class ReprocessorExporterTaskTypeUrlProvider
{
    public static string? Url(TaskType status) =>
        status switch
        {
            TaskType.SiteAddressAndContactDetails => PagePaths.AddressOfReprocessingSite,
            TaskType.WasteLicensesPermitsAndExemptions => PagePaths.WastePermitExemptions,
            TaskType.ReprocessingInputsAndOutputs => PagePaths.ReprocessingInputOutput,
            TaskType.SamplingAndInspectionPlan => PagePaths.RegistrationSamplingAndInspectionPlan,
            TaskType.Unknown => null,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
}

/// <summary>
/// Provides a means to retrieve the Url for a permit type.
/// </summary>
public static class ReprocessorExporterPermitTypeUrlProvider
{
    public static string? Url(PermitType? permitType) =>
        permitType switch
        {
            PermitType.WasteExemption => PagePaths.ExemptionReferences,
            PermitType.PollutionPreventionAndControlPermit => PagePaths.PpcPermit,
            PermitType.WasteManagementLicence => PagePaths.WasteManagementLicense,
            PermitType.InstallationPermit => PagePaths.InstallationPermit,
            PermitType.EnvironmentalPermitOrWasteManagementLicence => PagePaths.EnvironmentalPermitOrWasteManagementLicence,
            PermitType.None => null,
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(permitType), permitType, null)
        };
}