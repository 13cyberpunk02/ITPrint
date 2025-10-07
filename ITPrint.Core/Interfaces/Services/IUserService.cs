using ITPrint.Core.Enums;
using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(string email, string username, string password, string firstName, string lastName, UserRole role, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(Guid userId, string? email, string? username, string? firstName, string? lastName, UserRole? role, bool? isActive, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}