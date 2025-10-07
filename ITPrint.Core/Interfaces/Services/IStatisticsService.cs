using ITPrint.Core.Enums;
using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Services;

public interface IStatisticsService
{
    Task<IEnumerable<PrintStatistics>> GetUserStatisticsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrintStatistics>> GetPrinterStatisticsAsync(Guid printerId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrintStatistics>> GetOverallStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<Dictionary<PaperFormat, int>> GetPaperFormatUsageAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrintStatistics>> GetTopUsersAsync(int count, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrintStatistics>> GetTopPrintersAsync(int count, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
    Task RecordPrintJobStatisticsAsync(PrintJob job, CancellationToken cancellationToken = default);
    Task UpdateDailyStatisticsAsync(Guid? userId, Guid? printerId, PaperFormat? paperFormat, bool success, int pages, long dataSize, TimeSpan printTime, CancellationToken cancellationToken = default);
}