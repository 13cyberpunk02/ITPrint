using System.ComponentModel.DataAnnotations;
using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.Users;

public class UpdateUserDto
{
    [EmailAddress(ErrorMessage = "Неверный формат электронной почты")]
    public string? Email { get; set; }

    [MinLength(3, ErrorMessage = "Имя пользователя должен содержать 3 символов.")]
    [MaxLength(50, ErrorMessage = "Имя пользователя не может превышать 50 символов.")]
    public string? Username { get; set; }

    [MaxLength(100, ErrorMessage = "Имя не может превышать 100 символов.")]
    public string? FirstName { get; set; }

    [MaxLength(100, ErrorMessage = "Фамилия не может превышать 100 символов.")]
    public string? LastName { get; set; }

    public UserRole? Role { get; set; }

    public bool? IsActive { get; set; }
}