using ITPrint.Core.Enums;

namespace ITPrint.Core.Models;

public class Printer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string CupsName { get; set; } = string.Empty;  
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PrinterStatus Status { get; set; }
    public int Priority { get; set; } = 0;  
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastPrintedAt { get; set; }
    
    public ICollection<PrinterCapability> Capabilities { get; set; } = new List<PrinterCapability>();
    public ICollection<PrintJobPage> AssignedPages { get; set; } = new List<PrintJobPage>();
    public ICollection<PrinterQueueItem> QueueItems { get; set; } = new List<PrinterQueueItem>();
}