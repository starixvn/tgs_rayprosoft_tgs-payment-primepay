namespace Tgs.Payment.PrimePay
{
    public class DepositCompletedCallbackMessage : CallbackMessage
    {
        public AdjustmentInfo? Adjustment { get; init; }

        public BankInfo? DesignatedBank { get; init; }

        public decimal ServiceFee { get; init; }

        public decimal TransactionAmount { get; init; }
    }
}
