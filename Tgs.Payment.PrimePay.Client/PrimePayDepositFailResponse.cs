namespace Tgs.Payment.PrimePay
{
    public class PrimePayDepositFailResponse : PrimePayDepositResponse
    {
        public string Errors { get; init; } = default!;
    }
}
