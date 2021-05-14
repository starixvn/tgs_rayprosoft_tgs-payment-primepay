using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay.Converters
{
    internal class TransactionInfoJsonConverter : JsonConverter<TransactionInfo>
    {
        public override TransactionInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                (28, _) => JsonSerializer.Deserialize<TransferringTransactionInfo>(json, options),
                (1, "D") => JsonSerializer.Deserialize<DepositCompletedTransactionInfo>(json, options),
                (1, "W") => JsonSerializer.Deserialize<WithdrawCompletedTransactionInfo>(json, options),
                (4, _) => JsonSerializer.Deserialize<RefundedTransactionInfo>(json, options),
                _ => JsonSerializer.Deserialize<NormalTransactionInfo>(json, options)
            };
        }

        public override void Write(Utf8JsonWriter writer, TransactionInfo value, JsonSerializerOptions options)
        {
            var json = value switch
            {
                TransferringTransactionInfo transferring => JsonSerializer.Serialize(transferring, options),
                DepositCompletedTransactionInfo deposit => JsonSerializer.Serialize(deposit, options),
                WithdrawCompletedTransactionInfo withdraw => JsonSerializer.Serialize(withdraw, options),
                RefundedTransactionInfo refunded => JsonSerializer.Serialize(refunded, options),
                _ => JsonSerializer.Serialize((NormalTransactionInfo)value, options)
            };

            JsonDocument.Parse(json).WriteTo(writer);
        }
    }
}
