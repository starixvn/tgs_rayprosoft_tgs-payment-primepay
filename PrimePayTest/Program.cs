using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tgs.Payment.PrimePay;

namespace PrimePayTest
{
    internal class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddNatsMessageQueue(config => { });
                    services.AddPaymentClient();
                    services
                        .AddSingleton<PrimePaySerializer>();
                    services.AddHostedService<Demo>();
                });

        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}
