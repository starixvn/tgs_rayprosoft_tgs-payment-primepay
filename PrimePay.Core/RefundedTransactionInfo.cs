namespace Tgs.Payment.PrimePay;

internal class RefundedTransactionInfo : TransactionInfo
{
    public string RefundedReason { get; init; } = default!;
}
