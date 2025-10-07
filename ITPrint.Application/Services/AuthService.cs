using ITPrint.Core.Enums;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Interfaces.Services;
using ITPrint.Core.Models;
using Microsoft.Extensions.Logging;

namespace ITPrint.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    ITokenService tokenService,
    ILogger<AuthService> logger)
    : IAuthService
{
    public async  Task<(User User, string Token)?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("Попытка входа не удалась: пользователь с адресом электронной почты {Email} не найден", email);
                return null;
            }

            if (!user.IsActive)
            {
                logger.LogWarning("Попытка входа не удалась: пользователь {Email} неактивен", email);
                return null;
            }

            if (!await ValidatePasswordAsync(password, user.PasswordHash))
            {
                logger.LogWarning("Попытка входа не удалась: неверный пароль для пользователя {Email}", email);
                return null;
            }

            await userRepository.UpdateLastLoginAsync(user.Id, cancellationToken);
            
            var token = tokenService.GenerateAccessToken(user);

            logger.LogInformation("Пользователь {Email} успешно вошел в систему", email);

            return (user, token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при входе пользователя {Email}", email);
            throw;
        }
    }

    public async Task<User> RegisterAsync(string email, string username, string password, string firstName, string lastName,
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
            
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await userRepository.AddAsync(user, cancellationToken);

            logger.LogInformation("Зарегистрирован новый пользователь: {Email} ({Имя пользователя})", email, username);

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при регистрации пользователя для {Email}", email);
            throw;
        }
    }

    public Task<bool> ValidatePasswordAsync(string password, string passwordHash)
    {
        try
        {
            return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, passwordHash));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка проверки пароля");
            return Task.FromResult(false);
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await userRepository.GetByEmailAsync(email, cancellationToken);

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await userRepository.GetByIdAsync(userId, cancellationToken);
}