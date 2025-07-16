using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Epr.Reprocessor.Exporter.UI.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ModelStateExtensions
    {
        public static KeyValuePair<string, ModelStateEntry> GetModelStateEntry(this ModelStateDictionary modelState, string key)
        {
            return modelState.FirstOrDefault(x => x.Key == key);
        }
    }
}
