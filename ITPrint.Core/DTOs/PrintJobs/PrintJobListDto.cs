using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.PrintJobs;

public class PrintJobListDto
{
    public Guid Id { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public PrintJobStatus Status { get; set; }
    public int TotalPages { get; set; }
    public int Copies { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}