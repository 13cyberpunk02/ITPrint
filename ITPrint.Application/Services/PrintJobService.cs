using ITPrint.Core.Enums;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Interfaces.Services;
using ITPrint.Core.Models;
using Microsoft.Extensions.Logging;

namespace ITPrint.Application.Services;

public class PrintJobService(
    IPrintJobRepository printJobRepository,
    IUserRepository userRepository,
    ILogger<PrintJobService> logger)
    : IPrintJobService
{
    public async Task<PrintJob> CreatePrintJobAsync(Guid userId, string filePath, string fileName, long fileSizeBytes, int copies,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"Пользователь с идентификатором {userId} не найден");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException($"Пользователь {user.Email} не активен");
            }

            var printJob = new PrintJob
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OriginalFileName = fileName,
                OriginalFilePath = filePath,
                FileSizeBytes = fileSizeBytes,
                Status = PrintJobStatus.Uploaded,
                TotalPages = 0,
                Copies = copies,
                CreatedAt = DateTime.UtcNow
            };

            await printJobRepository.AddAsync(printJob, cancellationToken);

            logger.LogInformation(
                "Создано задание на печать: {JobId} для пользователя {UserId}, файл: {FileName}",
                printJob.Id, userId, fileName);

            return printJob;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка создания задания печати для пользователя {UserId}", userId);
            throw;
        }
    }

    public async Task<PrintJob?> GetPrintJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default)
        => await printJobRepository.GetByIdAsync(jobId, cancellationToken);

    public async Task<PrintJob?> GetPrintJobWithDetailsAsync(Guid jobId, CancellationToken cancellationToken = default)
        => await printJobRepository.GetJobWithPagesAsync(jobId, cancellationToken);

    public async Task<IEnumerable<PrintJob>> GetUserPrintJobsAsync(Guid userId, CancellationToken cancellationToken = default)
        => await printJobRepository.GetJobsByUserIdAsync(userId, cancellationToken);

    public async Task<IEnumerable<PrintJob>> GetAllPrintJobsAsync(CancellationToken cancellationToken = default)
        =>  await printJobRepository.GetAllAsync(cancellationToken);

    public async Task<IEnumerable<PrintJob>> GetRecentPrintJobsAsync(int count, CancellationToken cancellationToken = default)
        => await printJobRepository.GetRecentJobsAsync(count, cancellationToken);

    public async Task<PrintJob> UpdateJobStatusAsync(Guid jobId, PrintJobStatus status, string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var job = await printJobRepository.GetByIdAsync(jobId, cancellationToken);
            if (job == null)
            {
                throw new InvalidOperationException($"Задание на печать с идентификатором {jobId} не найдено");
            }

            var oldStatus = job.Status;
            await printJobRepository.UpdateStatusAsync(jobId, status, cancellationToken);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                job.ErrorMessage = errorMessage;
                await printJobRepository.UpdateAsync(job, cancellationToken);
            }

            logger.LogInformation(
                "Статус задания на печать {JobId} обновлен: {OldStatus} -> {NewStatus}",
                jobId, oldStatus, status);

            return job;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка обновления статуса задания печати {JobId}", jobId);
            throw;
        }
    }

    public async Task<bool> CancelPrintJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            var job = await printJobRepository.GetByIdAsync(jobId, cancellationToken);
            if (job == null)
            {
                return false;
            }

            if (job.Status is PrintJobStatus.Completed or PrintJobStatus.Cancelled or PrintJobStatus.Failed)
            {
                logger.LogWarning(
                    "Невозможно отменить задание на печать {JobId} со статусом {Status}",
                    jobId, job.Status);
                return false;
            }

            await printJobRepository.UpdateStatusAsync(jobId, PrintJobStatus.Cancelled, cancellationToken);

            logger.LogInformation("Задание печати {JobId} отменено", jobId);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка отмены задания печати {JobId}", jobId);
            throw;
        }
    }

    public async Task<bool> RetryPrintJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            var job = await printJobRepository.GetByIdAsync(jobId, cancellationToken);
            if (job == null)
            {
                return false;
            }

            if (job.Status != PrintJobStatus.Failed && job.Status != PrintJobStatus.PartiallyCompleted)
            {
                logger.LogWarning(
                    "Невозможно повторить задание на печать {JobId} со статусом {Status}",
                    jobId, job.Status);
                return false;
            }

            await printJobRepository.UpdateStatusAsync(jobId, PrintJobStatus.Uploaded, cancellationToken);

            job.ErrorMessage = null;
            job.ProcessingStartedAt = null;
            job.CompletedAt = null;
            await printJobRepository.UpdateAsync(job, cancellationToken);

            logger.LogInformation("Задание на печать {JobId} поставлено в очередь для повторной попытки", jobId);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при повторной попытке печати задания {JobId}", jobId);
            throw;
        }
    }

    public async Task<int> GetPendingJobsCountAsync(CancellationToken cancellationToken = default)
    {
        var statuses = new[]
        {
            PrintJobStatus.Uploaded,
            PrintJobStatus.Processing,
            PrintJobStatus.Splitting,
            PrintJobStatus.Routing,
            PrintJobStatus.InQueue,
            PrintJobStatus.Printing
        };

        var count = 0;
        foreach (var status in statuses)
        {
            var jobs = await printJobRepository.GetJobsByStatusAsync(status, cancellationToken);
            count += jobs.Count();
        }

        return count;
    }
}