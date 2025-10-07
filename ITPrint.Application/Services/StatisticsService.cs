using ITPrint.Core.Enums;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Interfaces.Services;
using ITPrint.Core.Models;
using Microsoft.Extensions.Logging;

namespace ITPrint.Application.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly ILogger<StatisticsService> _logger;

    public StatisticsService(
        IStatisticsRepository statisticsRepository,
        ILogger<StatisticsService> logger)
    {
        _statisticsRepository = statisticsRepository;
        _logger = logger;
    }
    public async Task<IEnumerable<PrintStatistics>> GetUserStatisticsAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null,
        CancellationToken cancellationToken = default)
        => await _statisticsRepository.GetUserStatisticsAsync(userId, startDate, endDate, cancellationToken);

    public async Task<IEnumerable<PrintStatistics>> GetPrinterStatisticsAsync(Guid printerId, DateTime? startDate = null, DateTime? endDate = null,
        CancellationToken cancellationToken = default)
        => await _statisticsRepository.GetPrinterStatisticsAsync(printerId, startDate, endDate, cancellationToken);

    public async Task<IEnumerable<PrintStatistics>> GetOverallStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null,
        CancellationToken cancellationToken = default)
        => await _statisticsRepository.GetOverallStatisticsAsync(startDate, endDate, cancellationToken);

    public async Task<Dictionary<PaperFormat, int>> GetPaperFormatUsageAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        => await _statisticsRepository.GetPaperFormatUsageAsync(startDate, endDate, cancellationToken);

    public async Task<IEnumerable<PrintStatistics>> GetTopUsersAsync(int count, DateTime? startDate = null, DateTime? endDate = null,
        CancellationToken cancellationToken = default)
        =>  await _statisticsRepository.GetTopUsersAsync(count, startDate, endDate, cancellationToken);

    public async Task<IEnumerable<PrintStatistics>> GetTopPrintersAsync(int count, DateTime? startDate = null, DateTime? endDate = null,
        CancellationToken cancellationToken = default)
        => await _statisticsRepository.GetTopPrintersAsync(count, startDate, endDate, cancellationToken);

    public async Task RecordPrintJobStatisticsAsync(PrintJob job, CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var success = job.Status == PrintJobStatus.Completed;
            var failed = job.Status == PrintJobStatus.Failed;
            var cancelled = job.Status == PrintJobStatus.Cancelled;

            var printTime = TimeSpan.Zero;
            if (job is { ProcessingStartedAt: not null, CompletedAt: not null })
            {
                printTime = job.CompletedAt.Value - job.ProcessingStartedAt.Value;
            }
            
            var overallStats = await _statisticsRepository.GetOrCreateDailyStatisticsAsync(
                null, null, null, today, cancellationToken);

            if (overallStats != null)
            {
                overallStats.TotalJobs++;
                overallStats.TotalPages += job.TotalPages;
                overallStats.TotalDataSizeBytes += job.FileSizeBytes;

                if (success) overallStats.SuccessfulJobs++;
                if (failed) overallStats.FailedJobs++;
                if (cancelled) overallStats.CancelledJobs++;

                if (printTime > TimeSpan.Zero)
                {
                    var totalTime = overallStats.AveragePrintTime.TotalSeconds * (overallStats.SuccessfulJobs - 1) + printTime.TotalSeconds;
                    overallStats.AveragePrintTime = TimeSpan.FromSeconds(totalTime / overallStats.SuccessfulJobs);
                }

                await _statisticsRepository.UpdateAsync(overallStats, cancellationToken);
            }

            var userStats = await _statisticsRepository.GetOrCreateDailyStatisticsAsync(
                job.UserId, null, null, today, cancellationToken);

            if (userStats != null)
            {
                userStats.TotalJobs++;
                userStats.TotalPages += job.TotalPages;
                userStats.TotalDataSizeBytes += job.FileSizeBytes;

                if (success) userStats.SuccessfulJobs++;
                if (failed) userStats.FailedJobs++;
                if (cancelled) userStats.CancelledJobs++;

                if (printTime > TimeSpan.Zero)
                {
                    var totalTime = userStats.AveragePrintTime.TotalSeconds * (userStats.SuccessfulJobs - 1) + printTime.TotalSeconds;
                    userStats.AveragePrintTime = TimeSpan.FromSeconds(totalTime / userStats.SuccessfulJobs);
                }

                await _statisticsRepository.UpdateAsync(userStats, cancellationToken);
            }

            _logger.LogInformation("Статистика, записана для задания на печать {JobId}", job.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при записи статистики для задания на печать {JobId}", job.Id);
            // Не выдавать исключение - запись статистики не должна нарушать основной рабочий процесс
        }
    }

    public async Task UpdateDailyStatisticsAsync(Guid? userId, Guid? printerId, PaperFormat? paperFormat, bool success, int pages,
        long dataSize, TimeSpan printTime, CancellationToken cancellationToken = default)
    {
        try
        {
            var today = DateTime.UtcNow.Date;

            var stats = await _statisticsRepository.GetOrCreateDailyStatisticsAsync(
                userId, printerId, paperFormat, today, cancellationToken);

            if (stats != null)
            {
                stats.TotalJobs++;
                stats.TotalPages += pages;
                stats.TotalDataSizeBytes += dataSize;

                if (success)
                {
                    stats.SuccessfulJobs++;
                    
                    if (printTime > TimeSpan.Zero)
                    {
                        var totalTime = stats.AveragePrintTime.TotalSeconds * (stats.SuccessfulJobs - 1) + printTime.TotalSeconds;
                        stats.AveragePrintTime = TimeSpan.FromSeconds(totalTime / stats.SuccessfulJobs);
                    }
                }
                else
                {
                    stats.FailedJobs++;
                }

                await _statisticsRepository.UpdateAsync(stats, cancellationToken);

                _logger.LogDebug(
                    "Ежедневная статистика обновлена для пользователя: {UserId}, принтера: {PrinterId}, формата: {Format}",
                    userId, printerId, paperFormat);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обновления ежедневной статистики");
            // Не выдавать исключение - запись статистики не должна нарушать основной рабочий процесс
        }
    }
}