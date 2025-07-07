using Microsoft.AspNetCore.Mvc.ViewFeatures;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Epr.Reprocessor.Exporter.UI.Helpers;

public static class TempDataHelper
{
    public static void Set<T>(this ITempDataDictionary tempData, string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key) || tempData == null)
        {
            return;
        }

        tempData[key] = SerialiseObject(value);
    }

    public static T Get<T>(this ITempDataDictionary tempData, string key)
    {
        if (string.IsNullOrWhiteSpace(key) || tempData == null)
        {
            return default(T);
        }
        tempData.TryGetValue(key, out var value);
        return value == null ? default(T) : GetDeserializeObject<T>(value.ToString());
    }

    private static string SerialiseObject<T>(T customType)
    {
        return JsonSerializer.Serialize(customType);
    }

    private static T GetDeserializeObject<T>(string value)
    {
        return !string.IsNullOrWhiteSpace(value) ? JsonSerializer.Deserialize<T>(value) : default(T);
    }
}
