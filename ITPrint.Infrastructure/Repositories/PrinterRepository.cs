using ITPrint.Core.Enums;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Models;
using ITPrint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITPrint.Infrastructure.Repositories;

public class PrinterRepository(ApplicationDbContext context) : BaseRepository<Printer>(context), IPrinterRepository
{
    public async Task<IEnumerable<Printer>> GetActivePrintersAsync(CancellationToken cancellationToken = default)
        => await DbSet
            .Where(p => p.IsActive && p.Status == PrinterStatus.Active)
            .Include(p => p.Capabilities)
            .OrderByDescending(p => p.Priority)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Printer>> GetPrintersByFormatAsync(PaperFormat format, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(p => p.IsActive && 
                        p.Status == PrinterStatus.Active &&
                        p.Capabilities.Any(c => c.PaperFormat == format))
            .Include(p => p.Capabilities)
            .Include(p => p.QueueItems.Where(qi => qi.Status == QueueItemStatus.Queued))
            .OrderByDescending(p => p.Priority)
            .ToListAsync(cancellationToken);

    public async Task<Printer?> GetPrinterWithCapabilitiesAsync(Guid printerId, CancellationToken cancellationToken = default)
        =>  await DbSet
            .Include(p => p.Capabilities)
            .Include(p => p.QueueItems)
            .FirstOrDefaultAsync(p => p.Id == printerId, cancellationToken);

    public async Task<Printer?> GetByCupsNameAsync(string cupsName, CancellationToken cancellationToken = default)
        => await DbSet
            .Include(p => p.Capabilities)
            .FirstOrDefaultAsync(p => p.CupsName == cupsName, cancellationToken);

    public async Task UpdateStatusAsync(Guid printerId, PrinterStatus status, CancellationToken cancellationToken = default)
    {
        var printer = await GetByIdAsync(printerId, cancellationToken);
        if (printer != null)
        {
            printer.Status = status;
            await UpdateAsync(printer, cancellationToken);
        }
    }
}