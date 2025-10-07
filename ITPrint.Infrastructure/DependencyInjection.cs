using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Infrastructure.Data;
using ITPrint.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITPrint.Infrastructure;

public static  class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, 
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("PrintManagement.Infrastructure"));
            
            // или для SQL Server - убери комментарий тут и закомментируй верхнее для postgres
            // options.UseSqlServer(connectionString,
            //     sqlOptions => sqlOptions.MigrationsAssembly("PrintManagement.Infrastructure"));
        });
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPrinterRepository, PrinterRepository>();
        services.AddScoped<IPrintJobRepository, PrintJobRepository>();
        services.AddScoped<IPrintJobPageRepository, PrintJobPageRepository>();
        services.AddScoped<IPrinterQueueItemRepository, PrinterQueueItemRepository>();
        services.AddScoped<IUserFileRepository, UserFileRepository>();
        services.AddScoped<IStatisticsRepository, StatisticsRepository>();

        return services;
    }
    
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await context.Database.MigrateAsync();
        
        
        await DbInitializer.SeedAsync(context);
    }
}