namespace ITPrint.Core.DTOs.Statistics;

public class TopPrinterDto
{
    public Guid PrinterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public int TotalJobs { get; set; }
}