using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay;

internal class BalanceInfo
{
    public decimal AvailableBalance { get; init; }

    public decimal BalanceBroughtForward { get; init; }

    public string Currency { get; init; } = default!;

    public decimal GivenCredit { get; init; }

    [JsonPropertyName("lastTransDate")]
    public DateTimeOffset? LastTranscationDate { get; init; }

    public decimal MerchantBalance { get; init; }

    public decimal TotalMemberDeposit { get; init; }

    public decimal TotalMemberWithdraw { get; init; }

    public decimal TotalMerchantSettlement { get; init; }

    public decimal TotalMerchantTopUp { get; init; }
}
