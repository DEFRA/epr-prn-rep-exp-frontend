using FrontendSchemeRegistration.UI.Resources;

namespace Epr.Reprocessor.Exporter.UI.Helpers;

public static class ErrorReportHelpers
{
    public static string GetErrorMessage(string errorCode)
    {
        return ErrorCodes.ResourceManager.GetString(errorCode) ?? errorCode;
    }
}
