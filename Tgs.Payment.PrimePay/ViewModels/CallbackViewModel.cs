namespace Tgs.Payment.PrimePay.ViewModels
{
    public class CallbackViewModel
    {
        public string MerchantId { get; init; } = default!;

        public string Message { get; init; } = default!;

        public string TransactionId { get; init; } = default!;
    }
}
