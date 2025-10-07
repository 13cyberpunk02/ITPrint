namespace ITPrint.Core.DTOs.Files;

public class UserFileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public bool CanBePrinted { get; set; }
}