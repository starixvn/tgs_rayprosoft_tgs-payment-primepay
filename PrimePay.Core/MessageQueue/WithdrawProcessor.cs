using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Protos;

namespace Tgs.Payment.PrimePay.MessageQueue
{
    internal class WithdrawProcessor : IMessageProcessor
    {
        private readonly PrimePayOptions _options;
        private readonly PrimePayClient _primePayClient;
        private readonly PrimePaySerializer _primePaySerializer;

        public WithdrawProcessor(PrimePaySerializer primePaySerializer, IOptions<PrimePayOptions> optionsAccessor, PrimePayClient primePayClient)
        {
            _primePaySerializer = primePaySerializer ?? throw new ArgumentNullException(nameof(primePaySerializer));
            _primePayClient = primePayClient ?? throw new ArgumentNullException(nameof(primePayClient));
            _options = optionsAccessor.Value;
        }

        public async ValueTask<byte[]> HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var request = WithdrawRequest.Parser.ParseFrom(data);
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

            if (withdrawResponse != null)
            {
                var applyInfo = new PrimePayWithdrawApplyInfo(settings, withdrawResponse);

                var msg = new WithdrawResponse
                {
                    Result = _primePaySerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
            else
            {
                var response = new PrimePayWithdrawFailResponse
                {
                    Status = "fail",
                    Errors = "response is null"
                };

                var applyInfo = new PrimePayWithdrawApplyInfo(settings, response);

                var msg = new WithdrawResponse
                {
                    Result = _primePaySerializer.Serialize(applyInfo)
                };

                return msg.ToByteArray();
            }
        }
    }
}
