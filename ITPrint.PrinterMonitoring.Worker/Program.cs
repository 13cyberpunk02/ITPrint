using ITPrint.Infrastructure;
using ITPrint.PrinterMonitoring.Worker;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/printer-monitoring-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient();

var host = builder.Build();
try
{
    Log.Information("Запуск сервиса по мониторингу принтера");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Сервис по мониторингу принтера неожиданно завершилась");
}
finally
{
    Log.CloseAndFlush();
}