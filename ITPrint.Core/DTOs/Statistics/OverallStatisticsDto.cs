namespace ITPrint.Core.DTOs.Statistics;

public class OverallStatisticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalPrinters { get; set; }
    public int ActivePrinters { get; set; }
    public int TotalPrintJobs { get; set; }
    public int TotalPages { get; set; }
    public int SuccessfulJobs { get; set; }
    public int FailedJobs { get; set; }
    public int CancelledJobs { get; set; }
    public int PendingJobs { get; set; }
    public long TotalDataSizeBytes { get; set; }
    public string TotalDataSizeFormatted { get; set; } = string.Empty;
    public TimeSpan AveragePrintTime { get; set; }
    public Dictionary<string, int> JobsByStatus { get; set; } = new();
    public Dictionary<string, int> PagesByFormat { get; set; } = new();
}