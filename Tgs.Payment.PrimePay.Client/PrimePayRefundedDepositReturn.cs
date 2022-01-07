namespace Tgs.Payment.PrimePay;

public class PrimePayRefundedDepositReturn : PrimePayReturn
{
    public string RefundedReason { get; init; } = default!;
}
