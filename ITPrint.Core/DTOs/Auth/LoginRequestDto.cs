using System.ComponentModel.DataAnnotations;

namespace ITPrint.Core.DTOs.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Требуется электронная почта")]
    [EmailAddress(ErrorMessage = "Неверный формат электронной почты")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется пароль")]
    [MinLength(6, ErrorMessage = "Пароль должен быть не меньше 6 символов.")]
    public string Password { get; set; } = string.Empty;
}