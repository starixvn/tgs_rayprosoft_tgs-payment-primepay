using Tgs.Payment.Clients;

namespace Tgs.Payment.PrimePay;

public record WithdrawFormInfo(
    Guid Id,
    string? UserId,
    UserType UserType,
    Guid BrandId,
    Guid? BrandPaymentId,
    decimal Amount,
    PaymentFlowState Status,
    Guid BankInfoId,
    string? BankInfo,
    PrimePayWithdrawApplyInfo? ApplyInfo,
    string? PaymentReturn = null);
