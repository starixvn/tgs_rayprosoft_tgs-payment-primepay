using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tgs.Payment.Clients;
using Tgs.Payment.PrimePay;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        _ = services.AddNatsMessageQueue(config => { });
        _ = services.AddPaymentClient();
        _ = services.AddSingleton<PrimePaySerializer>();
    });

using var app = builder.Build();

var paymentClient = app.Services.GetRequiredService<IPaymentClient>();
var serializer = app.Services.GetRequiredService<PrimePaySerializer>();

var settings = new PrimePayDepositSettings(
    "c3a77b3187759940",
    "018a716fafde1b7f",
    "2c956cdbfabc792a",
    "RMB",
    Guid.NewGuid(),
    "https://google.com");

_ = await paymentClient
    .DepositAsync("tgs.payment.primepay.deposit", 1000M, serializer.Serialize(settings))
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

_ = await paymentClient
    .WithdrawAsync("tgs.payment.primepay.withdraw", 500M, serializer.Serialize(withdrawSettings))
    .ConfigureAwait(false);
