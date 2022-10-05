using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Tgs.Payment.PrimePay.Converters;

namespace Tgs.Payment.PrimePay;

public class PrimePayCryptography
{
	private static readonly Aes _Aes;

	private static readonly JsonSerializerOptions? _JsonSerializerOptions = new()
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = {
			new DecimalJsonConverter(),
			new NullableDateTimeOffsetJsonConverter(),
			new DateTimeOffsetJsonConverter(),
			new TransactionInfoJsonConverter(),
			new PrimePayReturnJsonConverter(),
			new PrimePayDepositResponseJsonConverter(),
			new PrimePayWithdrawResponseJsonConverter()
		}
	};

	private readonly ILogger<PrimePayCryptography> _logger;

	static PrimePayCryptography()
	{
		_Aes = Aes.Create();
		_Aes.KeySize = 128;
	}

	public PrimePayCryptography(ILogger<PrimePayCryptography> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public virtual T? Decrypt<T>(string key, string iv, string encryptedData)
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
		_logger.LogInformation("Encrypt data: {json}", json);

		var jsonByteArray = Encoding.UTF8.GetBytes(json);

		using var encryptor = _Aes.CreateEncryptor(Encoding.ASCII.GetBytes(key), Encoding.ASCII.GetBytes(iv));

		var encryptedByteArray = encryptor.TransformFinalBlock(jsonByteArray, 0, jsonByteArray.Length);

		return Convert.ToBase64String(encryptedByteArray);
	}

	private static TData? DeserializeObject<TData>(string json)
		=> JsonSerializer.Deserialize<TData>(json, _JsonSerializerOptions);

	private static string SerializeObject<TData>(TData data)
		=> JsonSerializer.Serialize(data, _JsonSerializerOptions);
}
