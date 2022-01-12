using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Tgs.Payment.PrimePay;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        _ = services
            .AddPrimePay((options, sp) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                config.GetSection("PrimePay").Bind(options);

                var jsonOptions = sp.GetRequiredService<IOptions<JsonOptions>>().Value;

                options.SystemSerializerOptions = jsonOptions.JsonSerializerOptions;
            });

        _ = services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.AddPrimePayConverters();
            });
    }
}
