using ITPrint.FileStorage.Worker;
using ITPrint.Infrastructure;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/file-storage-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
try
{
    Log.Information("Запуск сервиса хранилища файлов");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Сервис хранилище файлов неожиданно завершилась");
}
finally
{
    Log.CloseAndFlush();
}