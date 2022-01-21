using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay.Converters;

public class PrimePayReturnJsonConverter : JsonConverter<PrimePayReturn>
{
    public override PrimePayReturn? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonDoc = JsonDocument.ParseValue(ref reader);

        var status = jsonDoc.RootElement.GetProperty("status").GetInt32();
        var transactionType = jsonDoc.RootElement.GetProperty("transactionType").GetString();

        using var ms = new MemoryStream();

        var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false });
        jsonDoc.WriteTo(writer);
        writer.Flush();

        var json = Encoding.UTF8.GetString(ms.ToArray());

        return (status, transactionType) switch
        {
            (28, _) => JsonSerializer.Deserialize<PrimePayTransferringDepositReturn>(json, options),
            (1, "D") => JsonSerializer.Deserialize<PrimePayDepositCompletedReturn>(json, options),
            (1, "W") => JsonSerializer.Deserialize<PrimePayWithdrawCompletedReturn>(json, options),
            (4, _) => JsonSerializer.Deserialize<PrimePayRefundedDepositReturn>(json, options),
            _ => JsonSerializer.Deserialize<PrimePayNormalDepositReturn>(json, options)
        };
    }

    public override void Write(Utf8JsonWriter writer, PrimePayReturn value, JsonSerializerOptions options)
    {
        var json = value switch
        {
            PrimePayTransferringDepositReturn transferring => JsonSerializer.Serialize(transferring, options),
            PrimePayDepositCompletedReturn deposit => JsonSerializer.Serialize(deposit, options),
            PrimePayWithdrawCompletedReturn withdraw => JsonSerializer.Serialize(withdraw, options),
            PrimePayRefundedDepositReturn refunded => JsonSerializer.Serialize(refunded, options),
            _ => JsonSerializer.Serialize((PrimePayNormalDepositReturn)value, options)
        };

        JsonDocument.Parse(json).WriteTo(writer);
    }
}
