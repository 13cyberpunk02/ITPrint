using ITPrint.Core.Enums;
using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Repositories;

public interface IStatisticsRepository : IBaseRepository<PrintStatistics>
{
    Task<IEnumerable<PrintStatistics>> GetUserStatisticsAsync(
        Guid userId, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PrintStatistics>> GetPrinterStatisticsAsync(
        Guid printerId, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PrintStatistics>> GetOverallStatisticsAsync(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        CancellationToken cancellationToken = default);
    
    Task<PrintStatistics?> GetOrCreateDailyStatisticsAsync(
        Guid? userId, 
        Guid? printerId, 
        PaperFormat? paperFormat, 
        DateTime date, 
        CancellationToken cancellationToken = default);
    
    Task<Dictionary<PaperFormat, int>> GetPaperFormatUsageAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PrintStatistics>> GetTopUsersAsync(
        int count, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<PrintStatistics>> GetTopPrintersAsync(
        int count, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        CancellationToken cancellationToken = default);
}