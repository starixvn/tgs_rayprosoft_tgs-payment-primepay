using Tgs.Payment.Clients;

namespace Tgs.Payment.PrimePay.Services;

public record SaveWithdrawReturnCommand(
    Guid FormId,
    PrimePayReturn WithdrawReturn,
    decimal? ActualAmount);
