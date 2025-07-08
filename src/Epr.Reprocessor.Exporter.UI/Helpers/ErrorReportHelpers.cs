using Epr.Reprocessor.Exporter.UI.Resources.Views.Shared;

namespace Epr.Reprocessor.Exporter.UI.Helpers;

public static class ErrorReportHelpers
{
    public static string GetErrorMessage(string errorCode)
    {
        return ErrorCodes.ResourceManager.GetString(errorCode) ?? errorCode;
    }
}
