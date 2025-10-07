using System.ComponentModel.DataAnnotations;

namespace ITPrint.Core.DTOs.Printers;

public class CreatePrinterDto
{
    [Required(ErrorMessage = "Требуется имя принтера")]
    [MaxLength(200, ErrorMessage = "Имя не может превышать 200 символов")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется модель")]
    [MaxLength(200, ErrorMessage = "Модель не может превышать 200 символов.")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "Требуется имя CUPS сервера")]
    [MaxLength(200, ErrorMessage = "Имя CUPS сервера не может превышать 200 символов")]
    public string CupsName { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Местоположение не может превышать 500 символов")]
    public string Location { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Описание не может превышать 1000 символов")]
    public string Description { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "Приоритет должен быть от 0 до 100")]
    public int Priority { get; set; } = 10;
}