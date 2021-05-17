using System;

namespace Tgs.Payment.PrimePay
{
    public record WithdrawFormInfo(
        Guid Id,
        string PlayerCode,
        Guid BrandId,
        Guid? BrandPaymentId,
        decimal Amount,
        int Status,
        Guid BankInfoId,
        string? BankInfo,
        PrimePayWithdrawApplyInfo? ApplyInfo,
        string? PaymentReturn = null);
}
