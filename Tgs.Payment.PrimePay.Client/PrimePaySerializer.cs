using System.Text.Json;
using Tgs.Payment.PrimePay.Converters;

namespace Tgs.Payment.PrimePay
{
    public class PrimePaySerializer
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            Converters = {
                new PrimePayDepositResponseJsonConverter(),
                new PrimePayWithdrawResponseJsonConverter()
            }
        };

        public T? Deserialize<T>(string json)
            => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);

        public string Serialize<T>(T data)
            => JsonSerializer.Serialize(data, _jsonSerializerOptions);
    }
}
