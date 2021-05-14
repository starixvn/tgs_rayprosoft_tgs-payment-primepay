namespace Tgs.Payment.PrimePay
{
    internal class DepositCompletedTransactionInfo : TransactionInfo
    {
        public string? Adjustment { get; init; }

        public BankInfo DesignatedBank { get; init; } = default!;

        public decimal ServiceFee { get; init; } = default!;

        public decimal TransactionAmount { get; init; } = default!;
    }
}
