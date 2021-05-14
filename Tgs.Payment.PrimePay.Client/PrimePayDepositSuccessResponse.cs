using System;
using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay
{
    public class PrimePayDepositSuccessResponse : PrimePayDepositResponse
    {
        public decimal DepositAmount { get; init; }

        public BankInfo DesignatedBank { get; init; } = default!;

        public string DesignatedBankRemark { get; init; } = default!;

        [JsonPropertyName("dateExpired")]
        public DateTimeOffset ExpiredDate { get; init; }

        public Uri RedirectUrl { get; init; } = default!;

        public string ReferenceId { get; init; } = default!;

        public int ReferenceNo { get; init; }
    }
}
