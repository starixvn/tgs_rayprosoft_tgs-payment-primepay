namespace Tgs.Payment.PrimePay
{
    public record PrimePayWithdrawApplyInfo(
        PrimePayWithdrawSettings Settings,
        PrimePayWithdrawResponse Response);
}
