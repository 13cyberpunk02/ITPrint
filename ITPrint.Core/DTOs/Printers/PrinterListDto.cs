using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.Printers;

public class PrinterListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public PrinterStatus Status { get; set; }
    public int Priority { get; set; }
    public bool IsActive { get; set; }
    public List<PaperFormat> SupportedFormats { get; set; } = [];
    public int CurrentQueueSize { get; set; }
    public DateTime? LastPrintedAt { get; set; }
}