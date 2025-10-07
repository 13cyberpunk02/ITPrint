using ITPrint.Infrastructure;
using ITPrint.Statistics.Worker;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/statistics-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

try
{
    Log.Information("Запуск сервиса по сбору статистики");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Сервис по сбору статистики неожиданно завершилась");
}
finally
{
    Log.CloseAndFlush();
}