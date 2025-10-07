using ITPrint.Core.Enums;

namespace ITPrint.Core.Models;

public class PrintStatistics
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }           
    public Guid? PrinterId { get; set; }        
    public PaperFormat? PaperFormat { get; set; } 
    public DateTime Date { get; set; }          
    public int TotalJobs { get; set; }
    public int TotalPages { get; set; }
    public int SuccessfulJobs { get; set; }
    public int FailedJobs { get; set; }
    public int CancelledJobs { get; set; }
    public long TotalDataSizeBytes { get; set; }
    public TimeSpan AveragePrintTime { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User? User { get; set; }
    public Printer? Printer { get; set; }
}