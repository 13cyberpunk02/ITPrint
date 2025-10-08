using System.Reflection;
using ITPrint.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ITPrint.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : DbContext(options)
{
    public DbSet<User> Users { get; set; } 
    public DbSet<Printer> Printers { get; set; }
    public DbSet<PrinterCapability> PrinterCapabilities { get; set; }
    public DbSet<PrinterQueueItem> PrinterQueueItems { get; set; }
    public DbSet<PrintJob> PrintJobs { get; set; }
    public DbSet<PrintStatistics> PrintStatistics { get; set; }
    public DbSet<PrintJobPage> PrintJobPages { get; set; }
    public DbSet<UserFile> UserFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}