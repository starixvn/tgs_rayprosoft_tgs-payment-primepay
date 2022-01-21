namespace Tgs.Payment.PrimePay;

public class PrimePayWithdrawCompletedReturn : PrimePayReturn
{
    public AdjustmentInfo? Adjustment { get; init; }

    public string BankTransactionId { get; init; } = default!;

    public string FromBankName { get; init; } = default!;

    public decimal ServiceFee { get; init; }

    public decimal TransactionAmount { get; init; }

    public decimal TransactionFee { get; init; }
}
