using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.Printers;

public class PrinterCapabilityDto
{
    public Guid Id { get; set; }
    public PaperFormat PaperFormat { get; set; }
    public bool ColorSupport { get; set; }
    public bool DuplexSupport { get; set; }
    public int MaxCopies { get; set; }
    public int MaxPagesPerJob { get; set; }
}