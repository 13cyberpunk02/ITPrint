using ITPrint.Core.Enums;

namespace ITPrint.Core.Models;

public class PrinterCapability
{
    public Guid Id { get; set; }
    public Guid PrinterId { get; set; }
    public PaperFormat PaperFormat { get; set; }
    public bool ColorSupport { get; set; }
    public bool DuplexSupport { get; set; }
    public int MaxCopies { get; set; } = 1;
    public int MaxPagesPerJob { get; set; } = 1000;
    
    public Printer Printer { get; set; } = null!;
}