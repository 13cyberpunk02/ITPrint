namespace ITPrint.Core.DTOs.Statistics;

public class UserStatisticsDto
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public int TotalJobs { get; set; }
    public int TotalPages { get; set; }
    public int SuccessfulJobs { get; set; }
    public int FailedJobs { get; set; }
    public int CancelledJobs { get; set; }
    public long TotalDataSizeBytes { get; set; }
    public string TotalDataSizeFormatted { get; set; } = string.Empty;
    public TimeSpan AveragePrintTime { get; set; }
    public DateTime? LastPrintAt { get; set; }
}