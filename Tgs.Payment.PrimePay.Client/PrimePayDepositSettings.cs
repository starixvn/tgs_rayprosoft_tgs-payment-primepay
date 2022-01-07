namespace Tgs.Payment.PrimePay;

public record PrimePayDepositSettings(
    string MerchantId,
    string Key,
    string IV,
    string Currency,
    Guid TransactionId,
    string ResponseUrl,
    string? PaymentMethod = null);
