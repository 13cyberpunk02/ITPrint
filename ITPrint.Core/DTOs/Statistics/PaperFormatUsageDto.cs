using ITPrint.Core.Enums;

namespace ITPrint.Core.DTOs.Statistics;

public class PaperFormatUsageDto
{
    public PaperFormat Format { get; set; }
    public int TotalPages { get; set; }
    public decimal Percentage { get; set; }
}