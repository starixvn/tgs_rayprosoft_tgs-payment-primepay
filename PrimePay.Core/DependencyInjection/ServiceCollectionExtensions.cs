using System.Reflection;
using Microsoft.Extensions.Options;
using Tgs.Payment.PrimePay;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPrimePay(
        this IServiceCollection services,
        Action<PrimePayOptions, IServiceProvider> configure,
        Func<IServiceProvider, DelegatingHandler> httpClientMessageHandlerFactory)
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
            .AddHttpMessageHandler(httpClientMessageHandlerFactory)
            .Services
            .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
            .AddPaymentClient()
            .AddPaymentServiceClient(httpClientBuilder => httpClientBuilder
                .ConfigureHttpClient((sp, http) =>
                {
                    var primePayOptions = sp.GetRequiredService<IOptions<PrimePayOptions>>().Value;

                    http.BaseAddress = primePayOptions.PaymentServiceBaseUrl;
                })
                .AddHttpMessageHandler(httpClientMessageHandlerFactory))
            .AddTransient<PrimePayCryptography>()
            .AddSingleton<PrimePaySerializer>();
}