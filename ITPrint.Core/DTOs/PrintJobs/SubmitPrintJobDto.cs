using System.ComponentModel.DataAnnotations;

namespace ITPrint.Core.DTOs.PrintJobs;

public class SubmitPrintJobDto
{
    [Required(ErrorMessage = "Укажите идентификатор файла")]
    public Guid FileId { get; set; }

    [Range(1, 100, ErrorMessage = "Количество копий должно быть от 1 до 100")]
    public int Copies { get; set; } = 1;
}