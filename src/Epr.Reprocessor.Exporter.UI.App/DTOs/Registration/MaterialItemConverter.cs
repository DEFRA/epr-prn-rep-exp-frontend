using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage]
public class MaterialItemConverter : JsonConverter<Material>
{
    public override Material Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrEmpty(value))
        {
            return Material.None;
        }

        if (value == "Paper/Board")
        {
            return Material.Paper;
        }

        return Enum.Parse<Material>(value);
    }

    public override void Write(Utf8JsonWriter writer, Material value, JsonSerializerOptions options)
    {
        var output = value is Material.Paper ? "Paper/Board" : value.ToString();

        writer.WriteStringValue(output);
    }
}