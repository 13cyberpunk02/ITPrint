using ITPrint.Core.Enums;

namespace ITPrint.Core.Models;

public class PrintJob
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string OriginalFilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public PrintJobStatus Status { get; set; }
    public int TotalPages { get; set; }
    public int Copies { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessingStartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    
    public User User { get; set; } = null!;
    public ICollection<PrintJobPage> Pages { get; set; } = new List<PrintJobPage>();
}