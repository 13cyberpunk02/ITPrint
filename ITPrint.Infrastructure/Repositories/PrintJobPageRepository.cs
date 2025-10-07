using ITPrint.Core.Enums;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Models;
using ITPrint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITPrint.Infrastructure.Repositories;

public class PrintJobPageRepository(ApplicationDbContext context) : BaseRepository<PrintJobPage>(context), IPrintJobPageRepository
{
    public async Task<IEnumerable<PrintJobPage>> GetPagesByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(pp => pp.PrintJobId == jobId)
            .Include(pp => pp.AssignedPrinter)
            .Include(pp => pp.QueueItem)
            .OrderBy(pp => pp.PageNumber)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PrintJobPage>> GetPagesByStatusAsync(PrintJobPageStatus status, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(pp => pp.Status == status)
            .Include(pp => pp.PrintJob)
            .OrderBy(pp => pp.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<PrintJobPage?> GetPageWithQueueItemAsync(Guid pageId, CancellationToken cancellationToken = default)
        => await DbSet
            .Include(pp => pp.QueueItem)
            .Include(pp => pp.AssignedPrinter)
            .FirstOrDefaultAsync(pp => pp.Id == pageId, cancellationToken);

    public async Task UpdateStatusAsync(Guid pageId, PrintJobPageStatus status, CancellationToken cancellationToken = default)
    {
        var page = await GetByIdAsync(pageId, cancellationToken);
        if (page != null)
        {
            page.Status = status;
            
            if (status == PrintJobPageStatus.Printed)
            {
                page.PrintedAt = DateTime.UtcNow;
            }

            await UpdateAsync(page, cancellationToken);
        }
    }
}