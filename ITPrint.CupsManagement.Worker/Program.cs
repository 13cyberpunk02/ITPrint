using ITPrint.CupsManagement.Worker;
using ITPrint.Infrastructure;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/cups-management-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient("CUPS", client =>
{
    var cupsUrl = builder.Configuration["CupsSettings:ServerUrl"] ?? "http://localhost:631";
    client.BaseAddress = new Uri(cupsUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

var host = builder.Build();

try
{
    Log.Information("Запуск сервиса по управлению CUPS сервером");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Сервис по управлению CUPS сервером неожиданно завершилась");
}
finally
{
    Log.CloseAndFlush();
}
