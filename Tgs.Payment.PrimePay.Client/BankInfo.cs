using System.Text.Json.Serialization;

namespace Tgs.Payment.PrimePay;

public class BankInfo
{
    [JsonPropertyName("bankAcctNum")]
    public string BankAccount { get; init; } = default!;

    [JsonPropertyName("bankAcctName")]
    public string BankAccountName { get; init; } = default!;

    public string BankBranch { get; init; } = default!;

    public string BankName { get; init; } = default!;
}
