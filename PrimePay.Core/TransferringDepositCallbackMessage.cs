using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay
{
    public class TransferringDepositCallbackMessage : CallbackMessage
    {
        [JsonPropertyName("fromBankAcctName")]
        public string FromBankAccountName { get; init; } = default!;

        public decimal ServiceFee { get; init; }

        public decimal TransactionAmount { get; init; }
    }
}
