using System;

namespace Tgs.Payment.PrimePay.Services
{
    public record SaveWithdrawReturnCommand(Guid FormId, PrimePayReturn WithdrawReturn);
}
