using ITPrint.Core.Enums;
using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Repositories;

public interface IPrintJobPageRepository : IBaseRepository<PrintJobPage>
{
    Task<IEnumerable<PrintJobPage>> GetPagesByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrintJobPage>> GetPagesByStatusAsync(PrintJobPageStatus status, CancellationToken cancellationToken = default);
    Task<PrintJobPage?> GetPageWithQueueItemAsync(Guid pageId, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid pageId, PrintJobPageStatus status, CancellationToken cancellationToken = default);
}