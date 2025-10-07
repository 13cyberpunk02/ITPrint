using ITPrint.Core.Enums;
using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Repositories;

public interface IPrinterQueueItemRepository : IBaseRepository<PrinterQueueItem>
{
    Task<IEnumerable<PrinterQueueItem>> GetQueueByPrinterIdAsync(Guid printerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrinterQueueItem>> GetQueuedItemsAsync(CancellationToken cancellationToken = default);
    Task<PrinterQueueItem?> GetNextInQueueAsync(Guid printerId, CancellationToken cancellationToken = default);
    Task<PrinterQueueItem?> GetByCupsJobIdAsync(string cupsJobId, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid queueItemId, QueueItemStatus status, CancellationToken cancellationToken = default);
    Task IncrementRetryCountAsync(Guid queueItemId, CancellationToken cancellationToken = default);
    Task ReorderQueueAsync(Guid printerId, CancellationToken cancellationToken = default);
    Task<int> GetQueuePositionAsync(Guid printerId, CancellationToken cancellationToken = default);
}