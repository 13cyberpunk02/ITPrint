using System.ComponentModel.DataAnnotations;
using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.Users;

public class CreateUserDto
{
    [Required(ErrorMessage = "Требуется электронная почта")]
    [EmailAddress(ErrorMessage = "Неверный формат электронной почты")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется имя пользователя")]
    [MinLength(3, ErrorMessage = "Имя пользователя должен содержать 3 символов.")]
    [MaxLength(50, ErrorMessage = "Имя пользователя не может превышать 50 символов.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется пароль")]
    [MinLength(6, ErrorMessage = "Пароль должен содержать не менее 6 символов.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется имя")]
    [MaxLength(100, ErrorMessage = "Имя не может превышать 100 символов.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется фамилия")]
    [MaxLength(100, ErrorMessage = "Фамилия не может превышать 100 символов.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется роль")]
    public UserRole Role { get; set; }
}