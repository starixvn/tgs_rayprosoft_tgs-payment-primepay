using System;

namespace Tgs.Payment.PrimePay
{
    public record DepositFormInfo(
        Guid Id,
        string PlayerCode,
        Guid BrandId,
        Guid BrandPaymentId,
        decimal Amount,
        int Status,
        PrimePayDepositApplyInfo? ApplyInfo,
        string? PaymentReturn = null);
}
