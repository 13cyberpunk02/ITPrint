using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.PrintJobs;

public class PrintJobDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public PrintJobStatus Status { get; set; }
    public int TotalPages { get; set; }
    public int Copies { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessingStartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public TimeSpan? ProcessingDuration { get; set; }
}