namespace Tgs.Payment.PrimePay
{
    public class PrimePayWithdrawSuccessResponse : PrimePayWithdrawResponse
    {
        public string ReferenceId { get; init; } = default!;
    }
}
