namespace Tgs.Payment.PrimePay
{
    public record PrimePayDepositApplyInfo(
        PrimePayDepositSettings Settings,
        PrimePayDepositResponse Response);
}
