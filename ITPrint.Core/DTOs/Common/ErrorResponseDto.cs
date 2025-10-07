namespace ITPrint.Core.DTOs.Common;

public class ErrorResponseDto
{
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}