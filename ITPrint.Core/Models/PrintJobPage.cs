using ITPrint.Core.Enums;

namespace ITPrint.Core.Models;

public class PrintJobPage
{
    public Guid Id { get; set; }
    public Guid PrintJobId { get; set; }
    public int PageNumber { get; set; }
    public PaperFormat PaperFormat { get; set; }
    public double WidthMm { get; set; }
    public double HeightMm { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public PrintJobPageStatus Status { get; set; }
    public Guid? AssignedPrinterId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PrintedAt { get; set; }
    public string? ErrorMessage { get; set; }
    
    public PrintJob PrintJob { get; set; } = null!;
    public Printer? AssignedPrinter { get; set; }
    public PrinterQueueItem? QueueItem { get; set; }
}