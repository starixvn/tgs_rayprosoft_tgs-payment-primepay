using Google.Protobuf;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Protos;

namespace Tgs.Payment.PrimePay.MessageQueue;

internal class DepositProcessor : IMessageProcessor, IMessageExceptionProcessor
{
    private readonly PrimePayOptions _options;
    private readonly PrimePayClient _primePayClient;
    private readonly PrimePaySerializer _primePaySerializer;

    public DepositProcessor(PrimePaySerializer primePaySerializer, PrimePayClient primePayClient, IOptions<PrimePayOptions> optionsAccessor)
    {
        _primePaySerializer = primePaySerializer ?? throw new ArgumentNullException(nameof(primePaySerializer));
        _primePayClient = primePayClient ?? throw new ArgumentNullException(nameof(primePayClient));
        _options = optionsAccessor.Value;
    }

    public async ValueTask<byte[]> HandleAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = DepositRequest.Parser.ParseFrom(data);
        var settings = _primePaySerializer.Deserialize<PrimePayDepositSettings>(request.ApplyInfo)!;

        var order = new DepositOrder
        {
            Amount = decimal.Parse(request.Amount),
            Callback = $"{_options.CallbackBaseUrl}/deposit/callback",
            Currency = settings.Currency,
            MerchantId = settings.MerchantId,
            TransactionId = GuidHelper.GuidToString(settings.TransactionId),
            Response = "https://google.com", // settings.ResponseUrl,
            PaymentMethod = settings.PaymentMethod ?? "offline"
        };

        var depositResponse = await _primePayClient
            .DepositAsync(settings.Key, settings.IV, order)
            .ConfigureAwait(false);

        if (depositResponse != null)
        {
            var applyInfo = new PrimePayDepositApplyInfo(settings, depositResponse);

            var msg = new DepositResponse
            {
                Result = _primePaySerializer.Serialize(applyInfo)
            };

            return msg.ToByteArray();
        }
        else
        {
            var response = new PrimePayDepositFailResponse
            {
                Status = "fail",
                Errors = "response is null"
            };

            var applyInfo = new PrimePayDepositApplyInfo(settings, response);

            var msg = new DepositResponse
            {
                Result = _primePaySerializer.Serialize(applyInfo)
            };

            return msg.ToByteArray();
        }
    }

    public ValueTask<byte[]> HandleExceptionAsync(byte[] data, Exception exception, CancellationToken cancellationToken = default)
    {
        var request = DepositRequest.Parser.ParseFrom(data);
        var settings = _primePaySerializer.Deserialize<PrimePayDepositSettings>(request.ApplyInfo)!;

        var response = new PrimePayDepositFailResponse
        {
            Status = "fail",
            Errors = exception.ToString()
        };

        var applyInfo = new PrimePayDepositApplyInfo(settings, response);

        var msg = new DepositResponse
        {
            Result = _primePaySerializer.Serialize(applyInfo)
        };

        return ValueTask.FromResult(msg.ToByteArray());
    }
}
