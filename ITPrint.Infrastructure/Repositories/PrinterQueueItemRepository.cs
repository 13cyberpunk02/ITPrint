using ITPrint.Core.Enums;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Models;
using ITPrint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITPrint.Infrastructure.Repositories;

public class PrinterQueueItemRepository(ApplicationDbContext context) : BaseRepository<PrinterQueueItem>(context), IPrinterQueueItemRepository
{
    public async Task<IEnumerable<PrinterQueueItem>> GetQueueByPrinterIdAsync(Guid printerId, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(qi => qi.PrinterId == printerId)
            .Include(qi => qi.PrintJobPage)
            .ThenInclude(pp => pp.PrintJob)
            .OrderBy(qi => qi.QueuePosition)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PrinterQueueItem>> GetQueuedItemsAsync(CancellationToken cancellationToken = default)
        => await DbSet
            .Where(qi => qi.Status == QueueItemStatus.Queued)
            .Include(qi => qi.Printer)
            .Include(qi => qi.PrintJobPage)
            .OrderBy(qi => qi.AddedToQueueAt)
            .ToListAsync(cancellationToken);

    public async Task<PrinterQueueItem?> GetNextInQueueAsync(Guid printerId, CancellationToken cancellationToken = default)
        =>  await DbSet
            .Where(qi => qi.PrinterId == printerId && qi.Status == QueueItemStatus.Queued)
            .Include(qi => qi.PrintJobPage)
            .OrderBy(qi => qi.QueuePosition)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PrinterQueueItem?> GetByCupsJobIdAsync(string cupsJobId, CancellationToken cancellationToken = default)
        => await DbSet
            .Include(qi => qi.PrintJobPage)
            .Include(qi => qi.Printer)
            .FirstOrDefaultAsync(qi => qi.CupsJobId == cupsJobId, cancellationToken);

    public async Task UpdateStatusAsync(Guid queueItemId, QueueItemStatus status, CancellationToken cancellationToken = default)
    {
        var queueItem = await GetByIdAsync(queueItemId, cancellationToken);
        if (queueItem != null)
        {
            queueItem.Status = status;

            switch (status)
            {
                case QueueItemStatus.Printing when queueItem.StartedAt == null:
                    queueItem.StartedAt = DateTime.UtcNow;
                    break;
                case QueueItemStatus.Completed:
                case QueueItemStatus.Failed:
                    queueItem.CompletedAt = DateTime.UtcNow;
                    break;
            }

            await UpdateAsync(queueItem, cancellationToken);
        }
    }

    public async Task IncrementRetryCountAsync(Guid queueItemId, CancellationToken cancellationToken = default)
    {
        var queueItem = await GetByIdAsync(queueItemId, cancellationToken);
        if (queueItem != null)
        {
            queueItem.RetryCount++;
            queueItem.Status = queueItem.RetryCount >= queueItem.MaxRetries 
                ? QueueItemStatus.Failed 
                : QueueItemStatus.Retry;
            
            await UpdateAsync(queueItem, cancellationToken);
        }
    }

    public async Task ReorderQueueAsync(Guid printerId, CancellationToken cancellationToken = default)
    {
        var queueItems = await DbSet
            .Where(qi => qi.PrinterId == printerId && qi.Status == QueueItemStatus.Queued)
            .OrderBy(qi => qi.AddedToQueueAt)
            .ToListAsync(cancellationToken);

        for (var i = 0; i < queueItems.Count; i++)
        {
            queueItems[i].QueuePosition = i + 1;
        }

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetQueuePositionAsync(Guid printerId, CancellationToken cancellationToken = default)
    {
        var maxPosition = await DbSet
            .Where(qi => qi.PrinterId == printerId && qi.Status == QueueItemStatus.Queued)
            .MaxAsync(qi => (int?)qi.QueuePosition, cancellationToken);

        return (maxPosition ?? 0) + 1;
    }
}