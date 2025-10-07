using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.Printers;

public class PrinterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string CupsName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PrinterStatus Status { get; set; }
    public int Priority { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastPrintedAt { get; set; }
}