using Epr.Reprocessor.Exporter.UI.App.Enums;

namespace Epr.Reprocessor.Exporter.UI.App.DTOs.Registration;

[ExcludeFromCodeCoverage]
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