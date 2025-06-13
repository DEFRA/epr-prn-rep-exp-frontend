using System.Reflection;
using Epr.Reprocessor.Exporter.UI.App.Attributes;

namespace Epr.Reprocessor.Exporter.UI.App.Enums;

/// <summary>
/// Defines extensions for <see cref="MaterialItem"/>.
/// </summary>
public static class MaterialItemExtensions
{
    /// <summary>
    /// Gets the value for the material to use based on the <see cref="MaterialLookupAttribute"/>, if a 'Value' is not provided using the lookup, then the default string of the enum member will be used.
    /// </summary>
    /// <param name="item">The material item.</param>
    /// <returns>The value to use</returns>
    public static string GetMaterialName(this MaterialItem item)
    {
        if (typeof(MaterialItem).GetField(item.ToString())!.GetCustomAttribute(typeof(MaterialLookupAttribute)) is MaterialLookupAttribute attribute)
        {
            if (!string.IsNullOrEmpty(attribute.Value))
            {
                return attribute.Value;
            }
        }

        return item.ToString();
    }
}