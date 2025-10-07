using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.Statistics;

public class PrinterStatisticsDto
{
    public Guid PrinterId { get; set; }
    public string PrinterName { get; set; } = string.Empty;
    public string PrinterModel { get; set; } = string.Empty;
    public PrinterStatus Status { get; set; }
    public int TotalJobs { get; set; }
    public int TotalPages { get; set; }
    public int SuccessfulJobs { get; set; }
    public int FailedJobs { get; set; }
    public TimeSpan AveragePrintTime { get; set; }
    public DateTime? LastPrintAt { get; set; }
    public Dictionary<string, int> PagesByFormat { get; set; } = new();
}