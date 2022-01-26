namespace Tgs.Payment.PrimePay.Services;

public record SaveDepositReturnCommand(Guid FormId, PrimePayReturn DepositReturn);
