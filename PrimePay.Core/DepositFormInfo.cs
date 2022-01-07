using Tgs.Payment.Clients;

namespace Tgs.Payment.PrimePay;

public record DepositFormInfo(
    Guid Id,
    string PlayerCode,
    Guid BrandId,
    Guid BrandPaymentId,
    decimal Amount,
    PaymentFlowState Status,
    PrimePayDepositApplyInfo? ApplyInfo,
    string? PaymentReturn = null);
