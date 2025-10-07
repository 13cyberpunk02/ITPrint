using System.ComponentModel.DataAnnotations;
using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.Printers;

public class UpdatePrinterDto
{
    [MaxLength(200, ErrorMessage = "Имя не может превышать 200 символов")]
    public string? Name { get; set; }

    [MaxLength(200, ErrorMessage = "Модель не может превышать 200 символов")]
    public string? Model { get; set; }

    [MaxLength(500, ErrorMessage = "Местоположение не может превышать 500 символов")]
    public string? Location { get; set; }

    [MaxLength(1000, ErrorMessage = "Описание не может превышать 1000 символов")]
    public string? Description { get; set; }

    [Range(0, 100, ErrorMessage = "Приоритет должен быть от 0 до 100")]
    public int? Priority { get; set; }

    public PrinterStatus? Status { get; set; }

    public bool? IsActive { get; set; }
}