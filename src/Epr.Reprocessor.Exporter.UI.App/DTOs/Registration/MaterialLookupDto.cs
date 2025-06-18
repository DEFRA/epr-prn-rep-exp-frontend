using Epr.Reprocessor.Exporter.UI.App.Enums;
using Epr.Reprocessor.Exporter.UI.App.Extensions;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

/// <summary>
/// Defines a lookup dto for the details of a singular material including its name and ID, this is tied to a <see cref="RegistrationMaterialDto.MaterialLookup"/> which defines the details of the material lookup item that is associated with this registration for a material.
/// </summary>
[ExcludeFromCodeCoverage]
public record MaterialLookupDto
{
    /// <summary>
    /// The name of the material.
    /// </summary>
    [JsonConverter(typeof(MaterialItemConverter))]
    public MaterialItem Name { get; set; }

    /// <summary>
    /// The id of the entry, used to tie entries back together.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The shorthand code for the material.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// The display text for the material to be displayed on screen.
    /// </summary>
    public string DisplayText => Name.GetDisplayName();
}

public class MaterialItemConverter : JsonConverter<MaterialItem>
{
    public override MaterialItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrEmpty(value))
        {
            return MaterialItem.None;
        }

        if (value == "Paper/Board")
        {
            return MaterialItem.Paper;
        }

        return Enum.Parse<MaterialItem>(value);
    }

    public override void Write(Utf8JsonWriter writer, MaterialItem value, JsonSerializerOptions options)
    {
        var output = value is MaterialItem.Paper ? "Paper/Board" : value.ToString();

        writer.WriteStringValue(output);
    }
}
