namespace ITPrint.Core.DTOs.Files;

public class FileUploadResponseDto
{
    public Guid FileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string Message { get; set; } = "Файл успешно загружен";
}