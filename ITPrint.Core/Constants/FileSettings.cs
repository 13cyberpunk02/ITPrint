namespace ITPrint.Core.Constants;

public static class FileSettings
{
    // Максимальный размер файла в байтах (100 MB)
    public const long MaxFileSize = 100 * 1024 * 1024;
    
    // Поддерживаемые форматы файлов
    public static readonly string[] AllowedExtensions =
    [
        ".pdf",
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff",
        ".doc", ".docx",
        ".xls", ".xlsx",
        ".ppt", ".pptx"
    ];
    
    // MIME типы
    public static readonly Dictionary<string, string> MimeTypes = new()
    {
        { ".pdf", "application/pdf" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".png", "image/png" },
        { ".gif", "image/gif" },
        { ".bmp", "image/bmp" },
        { ".tiff", "image/tiff" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".ppt", "application/vnd.ms-powerpoint" },
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" }
    };
    
    // Базовая директория для хранения файлов
    public const string BaseStoragePath = "storage";
    public const string UsersDirectory = "users";
    public const string JobsDirectory = "jobs";
    public const string TempDirectory = "temp";
}