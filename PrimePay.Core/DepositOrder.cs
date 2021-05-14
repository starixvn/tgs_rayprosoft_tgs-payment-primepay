using System;
using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay
{
    internal class DepositOrder
    {
        public decimal Amount { get; init; }

        [JsonPropertyName("bankAcctNum")]
        public string? BankAccount { get; init; }

        [JsonPropertyName("bankAcctName")]
        public string? BankAccountName { get; init; }

        public string? BankBranch { get; init; }

        public string? BankName { get; init; }

        public string Callback { get; init; } = default!;

        public string Currency { get; init; } = default!;

        public string? DesignatedBankCode { get; init; }

        public string MerchantId { get; init; } = default!;

        [JsonPropertyName("payMethod")]
        public string PaymentMethod { get; init; } = default!;

        public string Response { get; init; } = default!;

        public string TransactionId { get; init; } = default!;

        public string TransactionType => "D";
    }
}
