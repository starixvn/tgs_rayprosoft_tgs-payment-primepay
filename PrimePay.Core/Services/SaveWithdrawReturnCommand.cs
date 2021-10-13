using System;
using Tgs.Payment.Clients;

namespace Tgs.Payment.PrimePay.Services
{
    public record SaveWithdrawReturnCommand(
        Guid FormId,
        PaymentFlowState State,
        PrimePayReturn WithdrawReturn,
        decimal? ActualAmount);
}
