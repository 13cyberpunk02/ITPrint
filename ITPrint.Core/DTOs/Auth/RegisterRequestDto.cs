using System.ComponentModel.DataAnnotations;

namespace ITPrint.Core.DTOs.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Требуется адрес электронной почты.")]
    [EmailAddress(ErrorMessage = "Неверный формат электронной почты")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется имя пользователя")]
    [MinLength(3, ErrorMessage = "Имя пользователя должен содержать не менее 3 символов.")]
    [MaxLength(50, ErrorMessage = "Имя пользователя не может превышать 50 символов.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется пароль")]
    [MinLength(6, ErrorMessage = "Пароль должен содержать не менее 6 символов.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Пароль должен содержать как минимум одну заглавную букву, одну строчную букву, одну цифру и один специальный символ.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется имя")]
    [MaxLength(100, ErrorMessage = "Имя не может превышать 100 символов.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется фамилия")]
    [MaxLength(100, ErrorMessage = "Фамилия не может превышать 100 символов.")]
    public string LastName { get; set; } = string.Empty;
}