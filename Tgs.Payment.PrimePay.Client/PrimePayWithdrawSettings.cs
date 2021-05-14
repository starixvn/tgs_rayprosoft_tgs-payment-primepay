using System;

namespace Tgs.Payment.PrimePay
{
    public record PrimePayWithdrawSettings(
        string MerchantId,
        string Key,
        string IV,
        Guid TransactionId,
        string Currency,
        string BankAccount,
        string BankAccountName,
        string BankBranch,
        string BankName);
}
