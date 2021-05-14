namespace Tgs.Payment.PrimePay
{
    internal class ApiRequestBody
    {
        public string MerchantId { get; init; } = default!;

        public string Message { get; init; } = default!;
    }
}
