using System.ComponentModel.DataAnnotations;

namespace ITPrint.Core.DTOs.Auth;

public class ChangePasswordRequestDto
{
    [Required(ErrorMessage = "Требуется текущий пароль")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется новый пароль")]
    [MinLength(6, ErrorMessage = "Пароль должен содержать не менее 6 символов.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Пароль должен содержать как минимум одну заглавную букву, одну строчную букву, одну цифру и один специальный символ.")]
    public string NewPassword { get; set; } = string.Empty;
}