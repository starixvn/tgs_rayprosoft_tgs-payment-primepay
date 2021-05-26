using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tgs.Payment.PrimePay
{
    public class PrimePayCryptography
    {
        private readonly static Aes _Aes;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;
        private readonly ILogger<PrimePayCryptography> _logger;

        static PrimePayCryptography()
        {
            _Aes = Aes.Create();
            _Aes.KeySize = 128;
        }

        public PrimePayCryptography(IOptions<PrimePayOptions> optionsAccessor, ILogger<PrimePayCryptography> logger)
        {
            _jsonSerializerOptions = optionsAccessor.Value.SystemSerializerOptions;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public T? Decrypt<T>(string key, string iv, string encryptedData)
        {
            var encryptedByteArray = Convert.FromBase64String(encryptedData);

            using var decryptor = _Aes.CreateDecryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(iv));

            var decryptedByteArray = decryptor.TransformFinalBlock(encryptedByteArray, 0, encryptedByteArray.Length);

            var json = Encoding.UTF8.GetString(decryptedByteArray);

            return DeserializeObject<T>(json);
        }

        public string Encrypt<T>(string key, string iv, T data)
        {
            var json = SerializeObject(data);
            _logger.LogInformation($"Encrypt data: {json}");

            var jsonByteArray = Encoding.UTF8.GetBytes(json);

            using var encryptor = _Aes.CreateEncryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(iv));

            var encryptedByteArray = encryptor.TransformFinalBlock(jsonByteArray, 0, jsonByteArray.Length);

            return Convert.ToBase64String(encryptedByteArray);
        }

        private TData? DeserializeObject<TData>(string json)
            => JsonSerializer.Deserialize<TData>(json, _jsonSerializerOptions);

        private string SerializeObject<TData>(TData data)
            => JsonSerializer.Serialize(data, _jsonSerializerOptions);
    }
}
