namespace Tgs.Payment.PrimePay;

public class PrimePayDepositCompletedReturn : PrimePayReturn
{
    public AdjustmentInfo? Adjustment { get; init; }

    public BankInfo? DesignatedBank { get; init; }

    public decimal ServiceFee { get; init; }

    public decimal TransactionAmount { get; init; }
}
