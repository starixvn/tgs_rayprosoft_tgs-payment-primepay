using System.Reflection;
using Microsoft.Extensions.Options;
using Tgs.Payment.PrimePay;
using Tgs.Payment.PrimePay.MessageQueue;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPrimePay(this IServiceCollection services, Action<PrimePayOptions, IServiceProvider> configure)
        => services
            .AddOptions<PrimePayOptions>()
            .Configure(configure)
            .Services
            .AddHttpClient<PrimePayClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<PrimePayOptions>>().Value;

                httpClient.BaseAddress = options.PaymentApiUrl;
            })
            .Services
            .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
            .AddTransient<PrimePayCryptography>()
            .AddSingleton<PrimePaySerializer>()
            .AddMessageQueue()
            .AddNatsMessageQueue(config =>
            {
                _ = config
                    .ConfigQueueOptions((options, sp) =>
                    {
                        var primePayOptions = sp.GetRequiredService<IOptions<PrimePayOptions>>().Value;

                        options.Url = primePayOptions.NatsUrl;
                    })
                    .AddProcessor<DepositProcessor>("tgs.payment.primepay.deposit", "tgs.payment.primepay.deposit")
                    .AddProcessor<WithdrawProcessor>("tgs.payment.primepay.withdraw", "tgs.payment.primepay.withdraw");
            })
            .AddNatsGlobPatternExchange("*")
            .Services
            .AddPaymentClient()
            .AddPaymentServiceClient((options, sp) =>
            {
                var primePayOptions = sp.GetRequiredService<IOptions<PrimePayOptions>>().Value;

                options.PaymentServiceBaseUrl = primePayOptions.PaymentServiceBaseUrl;
            });
}
