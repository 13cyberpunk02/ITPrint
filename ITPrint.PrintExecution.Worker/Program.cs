using ITPrint.Infrastructure;
using ITPrint.PrintExecution.Worker;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/print-execution-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
try
{
    Log.Information("Запуск сервиса по запуску печати");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Сервис по запуску печати неожиданно завершилась");
}
finally
{
    Log.CloseAndFlush();
}