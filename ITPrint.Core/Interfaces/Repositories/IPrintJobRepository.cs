using ITPrint.Core.Enums;
using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Repositories;

public interface IPrintJobRepository : IBaseRepository<PrintJob>
{
    Task<IEnumerable<PrintJob>> GetJobsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrintJob>> GetJobsByStatusAsync(PrintJobStatus status, CancellationToken cancellationToken = default);
    Task<PrintJob?> GetJobWithPagesAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid jobId, PrintJobStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrintJob>> GetRecentJobsAsync(int count, CancellationToken cancellationToken = default);
}