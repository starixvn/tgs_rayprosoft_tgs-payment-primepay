using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.Payment.PrimePay;
using Tgs.Payment.PrimePay.MessageQueue;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP");
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME");
            fluentOptions.UseMessagePack();
        });
    });

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.AddPrimePayConverters();
    });

builder.Services
    .AddHttpClientLoggingHandler()
    .AddPrimePay(
        (options, _) => builder.Configuration
            .GetSection("PrimePay")
            .Bind(options),
        sp => sp.ResolveLoggingHandler<IOptions<JsonOptions>>())
    .AddMessageQueue()
    .AddNatsMessageQueue(config => config
        .ConfigQueueOptions((options, sp) =>
        {
            var primePayOptions = sp.GetRequiredService<IOptions<PrimePayOptions>>().Value;

            options.Url = primePayOptions.NatsUrl;
        })
        .AddProcessor(ProcessorTypes.DepositProcessorType, "tgs.payment.primepay.deposit", "tgs.payment.primepay.deposit")
        .AddProcessor(ProcessorTypes.WithdrawProcessorType, "tgs.payment.primepay.withdraw", "tgs.payment.primepay.withdraw"))
    .AddNatsGlobPatternExchange("*");

builder.Services
    .AddHttpLogging(logging =>
    {
        logging.LoggingFields = HttpLoggingFields.All;
        logging.MediaTypeOptions.AddText("application/x-www-form-urlencoded");
        logging.MediaTypeOptions.AddText("text/html");
    });

builder.Services
    .AddOptions<OtlpExporterOptions>()
    .Configure<IConfiguration>((options, conifg) =>
    {
        var nodeIp = conifg.GetValue<string>("NODE_IP");

        if (!string.IsNullOrWhiteSpace(nodeIp))
            options.Endpoint = new Uri($"http://{nodeIp}:4317");

        conifg.GetSection("Opentelemetry:Tracing").Bind(options);
    })
    .Services
    .AddOpenTelemetryTracing(builder => builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProfileViewExporter()
        .AddOtlpExporter())
    .Configure<AspNetCoreInstrumentationOptions>(options => options.Filter =
        httpContext => !httpContext.Request.Path.HasValue
            || !httpContext.Request.Path.StartsWithSegments("/swagger")
            && !httpContext.Request.Path.StartsWithSegments("/healthz")
            && !httpContext.Request.Path.StartsWithSegments("/metrics")
            && !new[] { "html", "ico", "png", "jpg" }.Any(
                ext => httpContext.Request.Path.Value.EndsWith($".{ext}")));

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpLogging();

app.MapControllers();

app.MapHealthChecks("/healthz");

app.Use(async (context, next) =>
{
    await next().ConfigureAwait(false);

    var logger = context.RequestServices.GetService<ILoggerFactory>()
        ?.CreateLogger("Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware");

    if (logger?.IsEnabled(LogLevel.Information) == true)
    {
        using var stream = new StreamReader(context.Request.Body);

        _ = await stream.ReadToEndAsync().ConfigureAwait(false);
    }
});

await app.RunAsync();
