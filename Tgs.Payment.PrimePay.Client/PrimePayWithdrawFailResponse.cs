namespace Tgs.Payment.PrimePay;

public class PrimePayWithdrawFailResponse : PrimePayWithdrawResponse
{
    public string Errors { get; init; } = default!;
}
