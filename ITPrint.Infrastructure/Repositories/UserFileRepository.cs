using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Models;
using ITPrint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITPrint.Infrastructure.Repositories;

public class UserFileRepository(ApplicationDbContext context) : BaseRepository<UserFile>(context), IUserFileRepository
{
    public async Task<IEnumerable<UserFile>> GetFilesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(uf => uf.UserId == userId)
            .OrderByDescending(uf => uf.UploadedAt)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<UserFile>> GetActiveFilesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(uf => uf.UserId == userId && !uf.IsDeleted)
            .OrderByDescending(uf => uf.UploadedAt)
            .ToListAsync(cancellationToken);

    public async Task<UserFile?> GetFileByPathAsync(string filePath, CancellationToken cancellationToken = default)
        => await DbSet
            .FirstOrDefaultAsync(uf => uf.FilePath == filePath, cancellationToken);

    public async Task<long> GetUserStorageSizeAsync(Guid userId, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(uf => uf.UserId == userId && !uf.IsDeleted)
            .SumAsync(uf => uf.FileSizeBytes, cancellationToken);

    public async Task SoftDeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var file = await GetByIdAsync(fileId, cancellationToken);
        if (file != null)
        {
            file.IsDeleted = true;
            file.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(file, cancellationToken);
        }
    }

    public async Task<IEnumerable<UserFile>> GetDeletedFilesOlderThanAsync(DateTime date, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(uf => uf.IsDeleted && uf.DeletedAt < date)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<UserFile>> GetFilesOlderThanAsync(DateTime date, CancellationToken cancellationToken = default)
        => await DbSet
            .Where(uf => uf.UploadedAt < date)
            .ToListAsync(cancellationToken);
}