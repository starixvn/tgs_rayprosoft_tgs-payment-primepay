using OpenTelemetry.Exporter.ProfileViewer.Filters;
using OpenTelemetry.Trace;
using Tgs.Payment.PrimePay;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(builder =>
{
    _ = builder.AddJsonFile("/config/appsettings.json", true);
});

builder.Logging.AddTgsLog();

var startup = new Startup();

startup.ConfigureServices(builder.Services);

builder.Services.AddOpenTelemetryTracing(tracing =>
{
    _ = tracing
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProfileViewExporter(builder =>
        {
            _ = builder
                .AddFilter(new FileExtensionFilter("html", "ico", "png", "jpg"))
                .AddFilter(new UrlPathFilter("/swagger"))
                .AddFilter(new UrlPathFilter("/health"));
        });
});
builder.Services.AddProfileViewer();

var app = builder.Build();

app.MapControllers();
app.MapProfileViewer();
app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hello World!");
});

await app.RunAsync();
