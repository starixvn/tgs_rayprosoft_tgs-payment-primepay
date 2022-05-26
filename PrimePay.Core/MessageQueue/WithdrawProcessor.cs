using Google.Protobuf;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;

namespace Tgs.Payment.PrimePay.MessageQueue;

internal class WithdrawProcessor : IMessageProcessor, IMessageExceptionProcessor
{
    private readonly PrimePayOptions _options;
    private readonly PrimePayClient _primePayClient;
    private readonly IPaymentClient _paymentClient;
    private readonly PrimePaySerializer _primePaySerializer;

    public WithdrawProcessor(
        PrimePaySerializer primePaySerializer,
        IOptions<PrimePayOptions> optionsAccessor,
        PrimePayClient primePayClient,
        IPaymentClient paymentClient)
    {
        _primePaySerializer = primePaySerializer ?? throw new ArgumentNullException(nameof(primePaySerializer));
        _primePayClient = primePayClient ?? throw new ArgumentNullException(nameof(primePayClient));
        _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
        _options = optionsAccessor.Value;
    }

    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var request = WithdrawRequest.Parser.ParseFrom(data.Span);
        var settings = _primePaySerializer.Deserialize<PrimePayWithdrawSettings>(request.ApplyInfo)!;

        var order = new WithdrawOrder
        {
            Amount = decimal.Parse(request.Amount),
            BankAccount = settings.BankAccount,
            BankAccountName = settings.BankAccountName,
            BankBranch = settings.BankBranch,
            BankName = settings.BankName,
            Currency = settings.Currency,
            MerchantId = settings.MerchantId,
            Callback = $"{_options.CallbackBaseUrl}/withdraw/callback",
            TransactionId = GuidHelper.GuidToString(settings.TransactionId)
        };

        var withdrawResponse = await _primePayClient
            .WithdrawAsync(settings.Key, settings.IV, order)
            .ConfigureAwait(false);

        if (withdrawResponse is null)
            throw new ApiException(System.Net.HttpStatusCode.InternalServerError, "Response is null.");

        var applyInfo = new PrimePayWithdrawApplyInfo(settings, withdrawResponse);

        var thirdPartyOrderNo = ((PrimePayWithdrawSuccessResponse)applyInfo.Response).ReferenceId;
        await _paymentClient
            .SaveWithdrawFormThirdPartyOrderNoAsync(settings.TransactionId, thirdPartyOrderNo)
            .ConfigureAwait(false);

        var msg = new WithdrawResponse
        {
            Result = _primePaySerializer.Serialize(applyInfo)
        };

        return msg.ToByteArray();
    }

    public ValueTask<ReadOnlyMemory<byte>> HandleExceptionAsync(ReadOnlyMemory<byte> data, Exception exception, CancellationToken cancellationToken = default)
    {
        var request = WithdrawRequest.Parser.ParseFrom(data.Span);
        var settings = _primePaySerializer.Deserialize<PrimePayWithdrawSettings>(request.ApplyInfo)!;

        var response = new PrimePayWithdrawFailResponse
        {
            Status = "fail",
            Errors = exception.ToString()
        };

        var applyInfo = new PrimePayWithdrawApplyInfo(settings, response);

        var msg = new WithdrawResponse
        {
            Result = _primePaySerializer.Serialize(applyInfo)
        };

        return ValueTask.FromResult<ReadOnlyMemory<byte>>(msg.ToByteArray());
    }
}
