
using ITPrint.API.Common.Extensions;
using ITPrint.API.Endpoints;
using ITPrint.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/api-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOpenApiExtension();

builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

var app = builder.Build();

try
{
    await app.Services.InitializeDatabaseAsync();
    Log.Information("База данных успешно инициализирована");
}
catch (Exception e)
{
    Log.Fatal(e, "Не удалось инициализировать базу данных");
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("IT Printing API");
        options.WithTheme(ScalarTheme.BluePlanet);
    });
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new
{
    Status = "Здоров",
    Timestamp = DateTime.UtcNow,
    Service = "ITPrint.API"
}));


app.MapAllEndpoints();

try
{
    Log.Information("Запуск API приложения ITPrint");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение неожиданно завершилось");
}
finally
{
    Log.CloseAndFlush();
}
