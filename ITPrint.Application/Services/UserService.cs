using ITPrint.Core.Enums;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Interfaces.Services;
using ITPrint.Core.Models;
using Microsoft.Extensions.Logging;

namespace ITPrint.Application.Services;

public class UserService(
    IUserRepository userRepository,
    ILogger<UserService> logger)
    : IUserService
{
    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        => await userRepository.GetAllAsync(cancellationToken);

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
        => await userRepository.GetActiveUsersAsync(cancellationToken);

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
        => await userRepository.GetByRoleAsync(role, cancellationToken);

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await userRepository.GetByIdAsync(userId, cancellationToken);
    public async Task<User> CreateUserAsync(string email, string username, string password, string firstName, string lastName, UserRole role,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (await userRepository.EmailExistsAsync(email, cancellationToken))
            {
                throw new InvalidOperationException($"Пользователь с адресом электронной почты {email} уже существует");
            }

            if (await userRepository.UsernameExistsAsync(username, cancellationToken))
            {
                throw new InvalidOperationException($"Имя пользователя {username} уже занято");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await userRepository.AddAsync(user, cancellationToken);

            logger.LogInformation("Пользователь создан: {Email} с ролью {Role}", email, role);

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка создания пользователя {Email}", email);
            throw;
        }
    }

    public async Task<User> UpdateUserAsync(Guid userId, string? email, string? username, string? firstName, string? lastName, UserRole? role,
        bool? isActive, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"Пользователь с идентификатором {userId} не найден");
            }
            
            if (!string.IsNullOrEmpty(email) && email != user.Email)
            {
                if (await userRepository.EmailExistsAsync(email, cancellationToken))
                {
                    throw new InvalidOperationException($"Электронная почта {email} уже используется");
                }
                user.Email = email;
            }

            if (!string.IsNullOrEmpty(username) && username != user.Username)
            {
                if (await userRepository.UsernameExistsAsync(username, cancellationToken))
                {
                    throw new InvalidOperationException($"Имя пользователя {username} уже занято");
                }
                user.Username = username;
            }

            if (!string.IsNullOrEmpty(firstName))
                user.FirstName = firstName;

            if (!string.IsNullOrEmpty(lastName))
                user.LastName = lastName;

            if (role.HasValue)
                user.Role = role.Value;

            if (isActive.HasValue)
                user.IsActive = isActive.Value;

            await userRepository.UpdateAsync(user, cancellationToken);

            logger.LogInformation("Пользователь обновлен: {UserId}", userId);

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка обновления пользователя {UserId}", userId);
            throw;
        }
    }

    public async Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"Пользователь с идентификатором {userId} не найден");
            }

            user.IsActive = false;
            await userRepository.UpdateAsync(user, cancellationToken);

            logger.LogInformation("Пользователь деактивирован: {UserId}", userId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка удаления пользователя {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                logger.LogWarning("Не удалось сменить пароль: неверный текущий пароль для пользователя {UserId}", userId);
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await userRepository.UpdateAsync(user, cancellationToken);

            logger.LogInformation("Пароль изменен для пользователя {UserId}", userId);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка смены пароля для пользователя {UserId}", userId);
            throw;
        }
    }
}