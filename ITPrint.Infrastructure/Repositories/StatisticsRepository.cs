using ITPrint.Core.Enums;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Models;
using ITPrint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITPrint.Infrastructure.Repositories;

public class StatisticsRepository(ApplicationDbContext context) : BaseRepository<PrintStatistics>(context), IStatisticsRepository
{
    public async Task<IEnumerable<PrintStatistics>> GetUserStatisticsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(ps => ps.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(ps => ps.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(ps => ps.Date <= endDate.Value);

        return await query
            .Include(ps => ps.Printer)
            .OrderByDescending(ps => ps.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PrintStatistics>> GetPrinterStatisticsAsync(Guid printerId, DateTime? startDate = null, DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(ps => ps.PrinterId == printerId);

        if (startDate.HasValue)
            query = query.Where(ps => ps.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(ps => ps.Date <= endDate.Value);

        return await query
            .Include(ps => ps.User)
            .OrderByDescending(ps => ps.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PrintStatistics>> GetOverallStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(ps => ps.UserId == null && ps.PrinterId == null);

        if (startDate.HasValue)
            query = query.Where(ps => ps.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(ps => ps.Date <= endDate.Value);

        return await query
            .OrderByDescending(ps => ps.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<PrintStatistics?> GetOrCreateDailyStatisticsAsync(Guid? userId, Guid? printerId, PaperFormat? paperFormat, DateTime date,
        CancellationToken cancellationToken = default)
    {
        var dateOnly = date.Date;

        var existing = await DbSet
            .FirstOrDefaultAsync(ps => 
                    ps.UserId == userId &&
                    ps.PrinterId == printerId &&
                    ps.PaperFormat == paperFormat &&
                    ps.Date == dateOnly, 
                cancellationToken);

        if (existing != null)
            return existing;

        var newStats = new PrintStatistics
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PrinterId = printerId,
            PaperFormat = paperFormat,
            Date = dateOnly,
            TotalJobs = 0,
            TotalPages = 0,
            SuccessfulJobs = 0,
            FailedJobs = 0,
            CancelledJobs = 0,
            TotalDataSizeBytes = 0,
            AveragePrintTime = TimeSpan.Zero,
            CreatedAt = DateTime.UtcNow
        };

        return await AddAsync(newStats, cancellationToken);
    }

    public async Task<Dictionary<PaperFormat, int>> GetPaperFormatUsageAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(ps => ps.Date >= startDate && ps.Date <= endDate && ps.PaperFormat != null)
            .GroupBy(ps => ps.PaperFormat!.Value)
            .Select(g => new { Format = g.Key, Count = g.Sum(ps => ps.TotalPages) })
            .ToDictionaryAsync(x => x.Format, x => x.Count, cancellationToken);

    public async Task<IEnumerable<PrintStatistics>> GetTopUsersAsync(int count, DateTime? startDate = null, DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(ps => ps.UserId != null);

        if (startDate.HasValue)
            query = query.Where(ps => ps.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(ps => ps.Date <= endDate.Value);

        return await query
            .GroupBy(ps => ps.UserId)
            .Select(g => new PrintStatistics
            {
                UserId = g.Key,
                TotalJobs = g.Sum(ps => ps.TotalJobs),
                TotalPages = g.Sum(ps => ps.TotalPages),
                User = g.First().User
            })
            .OrderByDescending(ps => ps.TotalPages)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PrintStatistics>> GetTopPrintersAsync(int count, DateTime? startDate = null, DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(ps => ps.PrinterId != null);

        if (startDate.HasValue)
            query = query.Where(ps => ps.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(ps => ps.Date <= endDate.Value);

        return await query
            .GroupBy(ps => ps.PrinterId)
            .Select(g => new PrintStatistics
            {
                PrinterId = g.Key,
                TotalJobs = g.Sum(ps => ps.TotalJobs),
                TotalPages = g.Sum(ps => ps.TotalPages),
                Printer = g.First().Printer
            })
            .OrderByDescending(ps => ps.TotalPages)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}