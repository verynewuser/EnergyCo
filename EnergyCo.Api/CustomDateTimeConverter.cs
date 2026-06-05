using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EnergyCo.Api;

/// <summary>
/// We need this because the sent dates are in "03-Apr-2020" format instead of ISO8601
/// </summary>
public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "dd-MMM-yyyy";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString()!, Format, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}