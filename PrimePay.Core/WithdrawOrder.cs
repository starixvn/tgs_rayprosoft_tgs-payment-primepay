using System;
using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay
{
    internal class WithdrawOrder
    {
        public decimal Amount { get; init; }

        [JsonPropertyName("bankAccNum")]
        public string BankAccount { get; init; } = default!;

        [JsonPropertyName("bankAcctName")]
        public string BankAccountName { get; init; } = default!;

        public string BankBranch { get; init; } = default!;

        public string BankName { get; init; } = default!;

        public string Callback { get; init; } = default!;

        public string Currency { get; init; } = default!;

        public string MerchantId { get; init; } = default!;

        public string? TransactionDesc { get; init; }

        public string TransactionId { get; init; } = default!;

        public string TransactionType => "W";

        public string? UserEmail { get; init; }

        public string? UserId { get; init; }

        public string? UserIp { get; init; }

        public string? UserName { get; init; }
    }
}
