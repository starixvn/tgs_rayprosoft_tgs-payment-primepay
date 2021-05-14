using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay.Converters
{
    public class PrimePayDepositResponseJsonConverter : JsonConverter<PrimePayDepositResponse>
    {
        public override PrimePayDepositResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                "success" => JsonSerializer.Deserialize<PrimePayDepositSuccessResponse>(json, options),
                _ => JsonSerializer.Deserialize<PrimePayDepositFailResponse>(json, options)
            };
        }

        public override void Write(Utf8JsonWriter writer, PrimePayDepositResponse value, JsonSerializerOptions options)
        {
            var json = value switch
            {
                PrimePayDepositSuccessResponse successResponse => JsonSerializer.Serialize(successResponse, options),
                _ => JsonSerializer.Serialize((PrimePayDepositFailResponse)value, options)
            };

            JsonDocument.Parse(json).WriteTo(writer);
        }
    }
}
