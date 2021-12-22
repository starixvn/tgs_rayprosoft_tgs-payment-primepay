using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.PrimePay.Services
{
    internal class GetDepositFormQueryService : IGetService<GetDepositFormQuery, DepositFormInfo?>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly PrimePaySerializer _primePaySerializer;

        public GetDepositFormQueryService(IPaymentServiceClient paymentServiceClient, PrimePaySerializer primePaySerializer)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _primePaySerializer = primePaySerializer ?? throw new ArgumentNullException(nameof(primePaySerializer));
        }

        public async ValueTask<DepositFormInfo?> GetAsync(
            GetDepositFormQuery query,
            CancellationToken cancellationToken = default)
        {
            var depositFormId = GuidHelper.StringToGuid(query.TransactionId);

            var data = await _paymentServiceClient
                .GetDepositFormAsync(depositFormId)
                .ConfigureAwait(false);

            return data == null
                ? null
                : new DepositFormInfo(
                    data.Id,
                    data.PlayerCode,
                    data.BrandId,
                    data.BrandPaymentId,
                    data.Amount,
                    data.Status,
                    string.IsNullOrEmpty(data.ApplyInfo)
                        ? null
                        : _primePaySerializer.Deserialize<PrimePayDepositApplyInfo>(data.ApplyInfo),
                    data.PaymentReturn);
        }
    }
}
