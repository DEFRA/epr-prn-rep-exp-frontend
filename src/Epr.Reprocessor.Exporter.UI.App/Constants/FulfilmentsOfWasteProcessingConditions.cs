using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Constants;

[ExcludeFromCodeCoverage]
public static class FulfilmentsOfWasteProcessingConditions
{
    public const string ConditionsFulfilledEvidenceUploadUnwanted = "All conditions fulfilled, evidence upload not wanted";
    public const string ConditionsFulfilledEvidenceUploadwanted   = "All conditions fulfilled but evidence upload wanted";
    public const string AllConditionsNotFulfilled = "All conditions not fulfilled";

    public static MeetConditionsOfExport FulfilmentFromDescription(string description)
    {
        switch (description)
        {
            case ConditionsFulfilledEvidenceUploadUnwanted:
                return MeetConditionsOfExport.ConditionsFulfilledEvidenceUploadUnwanted;
            case ConditionsFulfilledEvidenceUploadwanted:
                return MeetConditionsOfExport.ConditionsFulfilledEvidenceUploadwanted;
            default:
                return MeetConditionsOfExport.AllConditionsNotFulfilled;
        }
    }
}
