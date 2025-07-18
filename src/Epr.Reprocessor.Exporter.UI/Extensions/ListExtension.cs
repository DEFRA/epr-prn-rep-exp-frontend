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

        public static T? PreviousOrDefault<T>(this List<T?> list, T? value = default, Predicate<T>? match = null)
        {
            var index = match is not null ? list.FindLastIndex(match!) : list.IndexOf(value);
            return index > 0 ? list[index - 1] : default(T);
        }
    }
}