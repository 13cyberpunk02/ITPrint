using ITPrint.Core.Enums;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Models;
using ITPrint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITPrint.Infrastructure.Repositories;

public class PrintJobRepository(ApplicationDbContext context) : BaseRepository<PrintJob>(context), IPrintJobRepository
{
    
    public async Task<IEnumerable<PrintJob>> GetJobsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(pj => pj.UserId == userId)
            .Include(pj => pj.Pages)
            .OrderByDescending(pj => pj.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PrintJob>> GetJobsByStatusAsync(PrintJobStatus status, CancellationToken cancellationToken = default)
        => await DbSet  
            .Where(pj => pj.Status == status)
            .Include(pj => pj.Pages)
            .OrderBy(pj => pj.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<PrintJob?> GetJobWithPagesAsync(Guid jobId, CancellationToken cancellationToken = default)
        =>  await DbSet
            .Include(pj => pj.Pages)
            .ThenInclude(pp => pp.AssignedPrinter)
            .Include(pj => pj.User)
            .FirstOrDefaultAsync(pj => pj.Id == jobId, cancellationToken);

    public async Task UpdateStatusAsync(Guid jobId, PrintJobStatus status, CancellationToken cancellationToken = default)
    {
        var job = await GetByIdAsync(jobId, cancellationToken);
        if (job != null)
        {
            job.Status = status;
            
            switch (status)
            {
                case PrintJobStatus.Processing when job.ProcessingStartedAt == null:
                    job.ProcessingStartedAt = DateTime.UtcNow;
                    break;
                case PrintJobStatus.Completed:
                case PrintJobStatus.Failed:
                    job.CompletedAt = DateTime.UtcNow;
                    break;
            }

            await UpdateAsync(job, cancellationToken);
        }
    }

    public async Task<IEnumerable<PrintJob>> GetRecentJobsAsync(int count, CancellationToken cancellationToken = default)
        =>  await DbSet
            .Include(pj => pj.User)
            .Include(pj => pj.Pages)
            .OrderByDescending(pj => pj.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
}