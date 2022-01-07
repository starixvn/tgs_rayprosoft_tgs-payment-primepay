using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay.Converters;

internal class NullableDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();

        return string.IsNullOrEmpty(dateString) ? null : (DateTimeOffset?)DateTimeOffset.Parse(dateString);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        => writer.WriteStringValue(
            value?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
}
