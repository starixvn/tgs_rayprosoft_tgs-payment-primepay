namespace Tgs.Payment.PrimePay
{
    public record PrimePayBankInfo(
        string BankName,
        string BankAccount,
        string Name,
        string? BankBranch);
}
