using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay.Converters
{
    public class PrimePayWithdrawResponseJsonConverter : JsonConverter<PrimePayWithdrawResponse>
    {
        public override PrimePayWithdrawResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonDoc = JsonDocument.ParseValue(ref reader);

            var status = jsonDoc.RootElement.GetProperty("status").GetString();

            using var ms = new MemoryStream();

            var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false });
            jsonDoc.WriteTo(writer);
            writer.Flush();

            var json = Encoding.UTF8.GetString(ms.ToArray());

            return status switch
            {
                "success" => JsonSerializer.Deserialize<PrimePayWithdrawSuccessResponse>(json, options),
                _ => JsonSerializer.Deserialize<PrimePayWithdrawFailResponse>(json, options)
            };
        }

        public override void Write(Utf8JsonWriter writer, PrimePayWithdrawResponse value, JsonSerializerOptions options)
        {
            var json = value switch
            {
                PrimePayWithdrawSuccessResponse successResponse => JsonSerializer.Serialize(successResponse, options),
                _ => JsonSerializer.Serialize((PrimePayWithdrawFailResponse)value, options)
            };

            JsonDocument.Parse(json).WriteTo(writer);
        }
    }
}
