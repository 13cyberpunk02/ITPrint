using ITPrint.DocumentProcessing.Worker;
using ITPrint.Infrastructure;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/document-processing-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

try
{
    Log.Information("Запуск сервиса по обработке документов");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Сервис обработки документов неожиданно завершилась");
}
finally
{
    Log.CloseAndFlush();
}

