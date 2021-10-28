using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.ServiceUtility;

namespace Tgs.Payment.PrimePay.Services
{
    class SaveDepositReturnCommandService : ICommandService<SaveDepositReturnCommand>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly PrimePaySerializer _primePaySerializer;

        public SaveDepositReturnCommandService(
            IPaymentServiceClient paymentServiceClient,
            PrimePaySerializer primePaySerializer)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _primePaySerializer = primePaySerializer ?? throw new ArgumentNullException(nameof(primePaySerializer));
        }
        public async ValueTask ExecuteAsync(
            SaveDepositReturnCommand command,
            CancellationToken cancellationToken = default)
        {
            var formData = await _paymentServiceClient
                .GetDepositFormAsync(command.FormId)
                .ConfigureAwait(false);

            if (formData == null)
                return;

            await _paymentServiceClient
                .SubmitDepositFormStatusAsync(
                    command.FormId,
                    formData.Status,
                    _primePaySerializer.Serialize(command.DepositReturn))
                .ConfigureAwait(false);
        }
    }
}
