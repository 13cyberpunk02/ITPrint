using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Services;

public interface IAuthService
{
    Task<(User User, string Token)?> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<User> RegisterAsync(string email, string username, string password, string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<bool> ValidatePasswordAsync(string password, string passwordHash);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
}