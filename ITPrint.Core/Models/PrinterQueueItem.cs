using ITPrint.Core.Enums;

namespace ITPrint.Core.Models;

public class PrinterQueueItem
{
    public Guid Id { get; set; }
    public Guid PrinterId { get; set; }
    public Guid PrintJobPageId { get; set; }
    public int QueuePosition { get; set; }
    public QueueItemStatus Status { get; set; }
    public int RetryCount { get; set; } = 0;
    public int MaxRetries { get; set; } = 3;
    public DateTime AddedToQueueAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CupsJobId { get; set; } 
    public string? ErrorMessage { get; set; }
    
    public Printer Printer { get; set; } = null!;
    public PrintJobPage PrintJobPage { get; set; } = null!;
}