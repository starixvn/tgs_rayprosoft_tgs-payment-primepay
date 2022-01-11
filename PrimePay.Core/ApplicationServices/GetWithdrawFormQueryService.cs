using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.PrimePay.Services;

internal class GetWithdrawFormQueryService : IGetService<GetWithdrawFormQuery, WithdrawFormInfo?>
{
    private readonly IPaymentServiceClient _paymentServiceClient;
    private readonly PrimePaySerializer _primePaySerializer;

    public GetWithdrawFormQueryService(
        IPaymentServiceClient paymentServiceClient,
        PrimePaySerializer primePaySerializer)
    {
        _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        _primePaySerializer = primePaySerializer ?? throw new ArgumentNullException(nameof(primePaySerializer));
    }

    public async ValueTask<WithdrawFormInfo?> GetAsync(GetWithdrawFormQuery query, CancellationToken cancellationToken = default)
    {
        var withdrawFormId = GuidHelper.StringToGuid(query.TransactionId);

        var data = await _paymentServiceClient
            .GetWithdrawFormAsync(withdrawFormId)
            .ConfigureAwait(false);

        return data == null
            ? null
            : new WithdrawFormInfo(
                data.Id,
                data.UserId,
                data.UserType,
                data.BrandId,
                data.BrandPaymentId,
                data.Amount,
                data.Status,
                data.BankInfoId,
                data.BankInfo,
                string.IsNullOrWhiteSpace(data.ApplyInfo)
                    ? null
                    : _primePaySerializer.Deserialize<PrimePayWithdrawApplyInfo>(data.ApplyInfo));
    }
}
