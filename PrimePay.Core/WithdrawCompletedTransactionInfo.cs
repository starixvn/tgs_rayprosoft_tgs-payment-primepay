using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay
{
    internal class WithdrawCompletedTransactionInfo : TransactionInfo
    {
        public AdjustmentInfo? Adjustment { get; init; }

        public string BankTransactionId { get; init; } = default!;

        [JsonPropertyName("fromBankAcctName")]
        public string FromBankAccountName { get; init; } = default!;

        public decimal ServiceFee { get; init; } = default!;

        public decimal TransactionAmount { get; init; } = default!;

        public decimal TransactionFee { get; init; }
    }
}
