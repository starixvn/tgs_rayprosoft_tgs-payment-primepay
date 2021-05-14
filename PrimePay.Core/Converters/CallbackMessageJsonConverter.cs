using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay.Converters
{
    internal class CallbackMessageJsonConverter : JsonConverter<CallbackMessage>
    {
        public override CallbackMessage? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                (28, _) => JsonSerializer.Deserialize<TransferringDepositCallbackMessage>(json, options),
                (1, "D") => JsonSerializer.Deserialize<DepositCompletedCallbackMessage>(json, options),
                (1, "W") => JsonSerializer.Deserialize<WithdrawCompletedCallbackMessage>(json, options),
                (4, _) => JsonSerializer.Deserialize<RefundedDepositCallbackMessage>(json, options),
                _ => JsonSerializer.Deserialize<NormalDepositCallbackMessage>(json, options)
            };
        }

        public override void Write(Utf8JsonWriter writer, CallbackMessage value, JsonSerializerOptions options)
        {
            var json = value switch
            {
                TransferringDepositCallbackMessage transferring => JsonSerializer.Serialize(transferring, options),
                DepositCompletedCallbackMessage deposit => JsonSerializer.Serialize(deposit, options),
                WithdrawCompletedCallbackMessage withdraw => JsonSerializer.Serialize(withdraw, options),
                RefundedDepositCallbackMessage refunded => JsonSerializer.Serialize(refunded, options),
                _ => JsonSerializer.Serialize((NormalDepositCallbackMessage)value, options)
            };

            JsonDocument.Parse(json).WriteTo(writer);
        }
    }
}
