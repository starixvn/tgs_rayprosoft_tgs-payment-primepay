using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.Payment;
using Tgs.Payment.PrimePay;

namespace PrimePayTest
{
    internal class Demo : BackgroundService
    {
        private readonly ILogger<Demo> _logger;
        private readonly IPaymentClient _paymentClient;
        private readonly PrimePaySerializer _primePaySerializer;

        public Demo(IPaymentClient paymentClient, PrimePaySerializer primePaySerializer, ILogger<Demo> logger)
        {
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
            _primePaySerializer = primePaySerializer ?? throw new ArgumentNullException(nameof(primePaySerializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var settings = new PrimePayDepositSettings(
                "c3a77b3187759940",
                "018a716fafde1b7f",
                "2c956cdbfabc792a",
                "RMB",
                Guid.NewGuid(),
                "https://google.com");

            var paymentReturn = await _paymentClient
                .DepositAsync("tgs.payment.primepay.deposit", 1000M, _primePaySerializer.Serialize(settings))
                .ConfigureAwait(false);

            var withdrawSettings = new PrimePayWithdrawSettings(
                "c3a77b3187759940",
                "018a716fafde1b7f",
                "2c956cdbfabc792a",
                Guid.NewGuid(),
                "RMB",
                "111111111111",
                "Tester",
                "bbbb",
                "TestBank");

            var withdrawReturn = await _paymentClient
                .WithdrawAsync("tgs.payment.primepay.withdraw", 500M, _primePaySerializer.Serialize(withdrawSettings))
                .ConfigureAwait(false);
        }
    }
}
