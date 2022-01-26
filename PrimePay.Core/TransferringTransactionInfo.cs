using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay;

internal class TransferringTransactionInfo : TransactionInfo
{
    [JsonPropertyName("fromBankAcctName")]
    public string? FromBankAccountName { get; init; }

    public decimal? ServiceFee { get; init; }

    public decimal? TransactionAmount { get; init; }
}
