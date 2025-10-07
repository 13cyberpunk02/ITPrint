namespace ITPrint.Core.DTOs.Statistics;

public class TopUserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int TotalPages { get; set; }
    public int TotalJobs { get; set; }
}