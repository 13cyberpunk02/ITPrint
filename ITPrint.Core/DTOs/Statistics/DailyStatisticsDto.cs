namespace ITPrint.Core.DTOs.Statistics;

public class DailyStatisticsDto
{
    public DateTime Date { get; set; }
    public int TotalJobs { get; set; }
    public int TotalPages { get; set; }
    public int SuccessfulJobs { get; set; }
    public int FailedJobs { get; set; }
}