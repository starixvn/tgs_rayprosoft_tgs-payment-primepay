namespace Tgs.Payment.PrimePay
{
    public class RefundedDepositCallbackMessage : CallbackMessage
    {
        public string RefundedReason { get; init; } = default!;
    }
}
