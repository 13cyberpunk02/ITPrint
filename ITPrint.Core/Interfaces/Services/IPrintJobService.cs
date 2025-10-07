using ITPrint.Core.Enums;
using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Services;

public interface IPrintJobService
{
    Task<PrintJob> CreatePrintJobAsync(Guid userId, string filePath, string fileName, long fileSizeBytes, int copies, CancellationToken cancellationToken = default);
    Task<PrintJob?> GetPrintJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<PrintJob?> GetPrintJobWithDetailsAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrintJob>> GetUserPrintJobsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrintJob>> GetAllPrintJobsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PrintJob>> GetRecentPrintJobsAsync(int count, CancellationToken cancellationToken = default);
    Task<PrintJob> UpdateJobStatusAsync(Guid jobId, PrintJobStatus status, string? errorMessage = null, CancellationToken cancellationToken = default);
    Task<bool> CancelPrintJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<bool> RetryPrintJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<int> GetPendingJobsCountAsync(CancellationToken cancellationToken = default);
}