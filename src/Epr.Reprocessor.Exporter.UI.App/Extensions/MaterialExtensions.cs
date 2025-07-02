using System.Reflection;
using Epr.Reprocessor.Exporter.UI.App.Attributes;
using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.Extensions;

/// <summary>
/// Defines extension methods for <see cref="Material"/>.
/// </summary>
public static class MaterialExtensions
{
    /// <summary>
    /// Gets the material name using the lookup attribute.
    /// </summary>
    /// <param name="material"></param>
    /// <returns></returns>
    public static string GetMaterialName(this Material material)
    {
        if (typeof(Material)
                .GetField(material.ToString())?
                .GetCustomAttributes(typeof(MaterialLookupAttribute)).Single() is MaterialLookupAttribute attribute && !string.IsNullOrEmpty(attribute.Value))
        {
            return attribute.Value;
        }

        return material.ToString();
    }
}