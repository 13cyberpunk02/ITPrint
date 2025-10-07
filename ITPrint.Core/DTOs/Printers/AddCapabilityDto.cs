using System.ComponentModel.DataAnnotations;
using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.Printers;

public class AddCapabilityDto
{
    [Required(ErrorMessage = "Требуется формат бумаги")]
    public PaperFormat PaperFormat { get; set; }

    public bool ColorSupport { get; set; } = false;

    public bool DuplexSupport { get; set; } = false;

    [Range(1, 1000, ErrorMessage = "Максимальное количество копий должно быть от 1 до 1000")]
    public int MaxCopies { get; set; } = 1;

    [Range(1, 10000, ErrorMessage = "Максимальное количество страниц на задание должно быть от 1 до 10000")]
    public int MaxPagesPerJob { get; set; } = 1000;
}