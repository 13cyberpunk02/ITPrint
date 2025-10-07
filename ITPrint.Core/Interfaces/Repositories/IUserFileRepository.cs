using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Repositories;

public interface IUserFileRepository : IBaseRepository<UserFile>
{
    Task<IEnumerable<UserFile>> GetFilesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserFile>> GetActiveFilesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserFile?> GetFileByPathAsync(string filePath, CancellationToken cancellationToken = default);
    Task<long> GetUserStorageSizeAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserFile>> GetDeletedFilesOlderThanAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserFile>> GetFilesOlderThanAsync(DateTime date, CancellationToken cancellationToken = default);
}