using ITPrint.Core.Enums;
using ITPrint.Core.Models;

namespace ITPrint.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Users.Any())
            return;
        
        var adminUser = new User()
        {
            Id = Guid.NewGuid(),
            Email = "admin@itprint.ru",
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            FirstName = "System",
            LastName = "Administrator",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        var managerUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "manager@printmgmt.com",
            Username = "manager",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
            FirstName = "Print",
            LastName = "Manager",
            Role = UserRole.Manager,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var regularUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@printmgmt.com",
            Username = "user",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
            FirstName = "Regular",
            LastName = "User",
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.AddRange(adminUser, managerUser, regularUser);
        
         var printerA4 = new Printer
        {
            Id = Guid.NewGuid(),
            Name = "Office Printer A4",
            Model = "HP LaserJet Pro",
            CupsName = "hp_laserjet_a4",
            Location = "Office Room 101",
            Description = "Standard A4 printer for documents",
            Status = PrinterStatus.Active,
            Priority = 10,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var printerA3 = new Printer
        {
            Id = Guid.NewGuid(),
            Name = "Design Printer A3",
            Model = "Canon imagePROGRAF",
            CupsName = "canon_a3",
            Location = "Design Department",
            Description = "A3 printer for larger documents",
            Status = PrinterStatus.Active,
            Priority = 8,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var printerLargeFormat = new Printer
        {
            Id = Guid.NewGuid(),
            Name = "Large Format Plotter",
            Model = "HP DesignJet T830",
            CupsName = "hp_designjet_a0",
            Location = "Print Shop",
            Description = "Large format printer for posters and blueprints",
            Status = PrinterStatus.Active,
            Priority = 5,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Printers.AddRange(printerA4, printerA3, printerLargeFormat);

        
        var capabilities = new List<PrinterCapability>
        {
            new PrinterCapability
            {
                Id = Guid.NewGuid(),
                PrinterId = printerA4.Id,
                PaperFormat = PaperFormat.A4,
                ColorSupport = true,
                DuplexSupport = true,
                MaxCopies = 100,
                MaxPagesPerJob = 500
            },
         
            new PrinterCapability
            {
                Id = Guid.NewGuid(),
                PrinterId = printerA3.Id,
                PaperFormat = PaperFormat.A4,
                ColorSupport = true,
                DuplexSupport = true,
                MaxCopies = 50,
                MaxPagesPerJob = 300
            },
            new PrinterCapability
            {
                Id = Guid.NewGuid(),
                PrinterId = printerA3.Id,
                PaperFormat = PaperFormat.A3,
                ColorSupport = true,
                DuplexSupport = false,
                MaxCopies = 50,
                MaxPagesPerJob = 200
            },
            new PrinterCapability
            {
                Id = Guid.NewGuid(),
                PrinterId = printerLargeFormat.Id,
                PaperFormat = PaperFormat.A2,
                ColorSupport = true,
                DuplexSupport = false,
                MaxCopies = 10,
                MaxPagesPerJob = 50
            },
            new PrinterCapability
            {
                Id = Guid.NewGuid(),
                PrinterId = printerLargeFormat.Id,
                PaperFormat = PaperFormat.A1,
                ColorSupport = true,
                DuplexSupport = false,
                MaxCopies = 10,
                MaxPagesPerJob = 50
            },
            new PrinterCapability
            {
                Id = Guid.NewGuid(),
                PrinterId = printerLargeFormat.Id,
                PaperFormat = PaperFormat.A0,
                ColorSupport = true,
                DuplexSupport = false,
                MaxCopies = 5,
                MaxPagesPerJob = 20
            },
            new PrinterCapability
            {
                Id = Guid.NewGuid(),
                PrinterId = printerLargeFormat.Id,
                PaperFormat = PaperFormat.A1x2,
                ColorSupport = true,
                DuplexSupport = false,
                MaxCopies = 5,
                MaxPagesPerJob = 10
            },
            new PrinterCapability
            {
                Id = Guid.NewGuid(),
                PrinterId = printerLargeFormat.Id,
                PaperFormat = PaperFormat.A1x3,
                ColorSupport = true,
                DuplexSupport = false,
                MaxCopies = 3,
                MaxPagesPerJob = 5
            }
        };

        context.PrinterCapabilities.AddRange(capabilities);
        
        await context.SaveChangesAsync();
    }
}