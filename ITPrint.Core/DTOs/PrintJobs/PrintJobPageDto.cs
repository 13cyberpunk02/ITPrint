using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.PrintJobs;

public class PrintJobPageDto
{
    public Guid Id { get; set; }
    public int PageNumber { get; set; }
    public PaperFormat PaperFormat { get; set; }
    public double WidthMm { get; set; }
    public double HeightMm { get; set; }
    public PrintJobPageStatus Status { get; set; }
    public Guid? AssignedPrinterId { get; set; }
    public string? AssignedPrinterName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PrintedAt { get; set; }
    public string? ErrorMessage { get; set; }
}