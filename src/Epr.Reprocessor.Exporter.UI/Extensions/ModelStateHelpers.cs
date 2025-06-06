using Epr.Reprocessor.Exporter.UI.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Epr.Reprocessor.Exporter.UI.Extensions;

public static class ModelStateHelpers
{
    public static void AddFileUploadExceptionsToModelState(List<string> exceptionCodes, ModelStateDictionary modelState)
    {
        foreach (var exceptionCode in exceptionCodes)
        {
            modelState.AddModelError("file", ErrorReportHelpers.GetErrorMessage(exceptionCode));
        }
    }
}