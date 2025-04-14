using Epr.Reprocessor.Exporter.UI.App.Constants;
using System.Diagnostics.CodeAnalysis;

namespace Epr.Reprocessor.Exporter.UI.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ListExtension
    {
        public static void AddIfNotExists<T>(this List<T> source, T value)
        {
            if (!source.Contains(value))
            {
                source.Add(value);
            }
        }

        public static T? PreviousOrDefault<T>(this List<T?> list, T value)
        {
            var index = list.LastIndexOf(value);
            return index > 0 ? list[index - 1] : default(T);
        }
    }
}
