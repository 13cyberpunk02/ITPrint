using System.Security.Claims;
using ITPrint.Core.Constants;
using ITPrint.Core.DTOs.Common;
using ITPrint.Core.DTOs.Files;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Models;

namespace ITPrint.API.Endpoints;

public static class FilesEndpoint
{
    public static IEndpointRouteBuilder MapFilesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/files")
            .WithTags("Files")
            .WithOpenApi();

        group.MapPost("upload", UploadFile)
            .Produces<ApiResponseDto<FileUploadResponseDto>>(StatusCodes.Status201Created)
            .Produces<ErrorResponseDto>(StatusCodes.Status400BadRequest);

        group.MapGet("/", GetUserFiles)
            .Produces<ApiResponseDto<List<UserFileDto>>>();
        
        group.MapGet("{id:guid}", GetFileById)
            .Produces<ApiResponseDto<UserFileDto>>()
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapDelete("{id:guid}", DeleteFile)
            .Produces<ApiResponseDto<bool>>()
            .Produces(StatusCodes.Status404NotFound);
        
        return group;
    }

    private static async Task<IResult> UploadFile(
        IFormFile? file,
        IConfiguration configuration,
        HttpContext context,
        IUserFileRepository  userFileRepository
        )
    {
        try
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            if (file == null || file.Length == 0)
            {
                return Results.BadRequest(new ErrorResponseDto
                {
                    Message = "Файл не был передан"
                });
            }
            
            var maxFileSizeMb = int.Parse(configuration["FileStorage:MaxFileSizeMB"] ?? "100");
            var maxFileSizeBytes = maxFileSizeMb * 1024 * 1024;

            if (file.Length > maxFileSizeBytes)
            {
                return Results.BadRequest(new ErrorResponseDto
                {
                    Message = $"Размер файла превышает максимально допустимый размер {maxFileSizeMb} МБ"
                });
            }
            
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = FileSettings.AllowedExtensions;

            if (!allowedExtensions.Contains(extension))
            {
                return Results.BadRequest(new ErrorResponseDto
                {
                    Message = $"Тип файла {extension} недопустим. Разрешенные типы: {string.Join(", ", allowedExtensions)}"
                });
            }
            
            var basePath = configuration["FileStorage:BasePath"] ?? "./storage";
            var userDirectory = Path.Combine(basePath, "users", userId.ToString());
            Directory.CreateDirectory(userDirectory);
            
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(userDirectory, uniqueFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            
            FileSettings.MimeTypes.TryGetValue(extension, out var mimeType);
            mimeType ??= "application/octet-stream";
            
            var userFile = new UserFile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FileName = file.FileName,
                FilePath = filePath,
                FileSizeBytes = file.Length,
                MimeType = mimeType,
                UploadedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await userFileRepository.AddAsync(userFile);
            var response = new FileUploadResponseDto
            {
                FileId = userFile.Id,
                FileName = userFile.FileName,
                FileSizeBytes = userFile.FileSizeBytes,
                FileSizeFormatted = FormatFileSize(userFile.FileSizeBytes),
                MimeType = userFile.MimeType,
                UploadedAt = userFile.UploadedAt
            };

            return Results.Created(userFile.Id.ToString(),
                ApiResponseDto<FileUploadResponseDto>.SuccessResult(response, "Файл загружен успешно")
            );
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при загрузке файла"
            });
        }
    }

    private static async Task<IResult> GetUserFiles(
        HttpContext context,
        IUserFileRepository  fileRepository
        )
    {
        try
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var files = await fileRepository.GetActiveFilesByUserIdAsync(userId);
            var fileDtos = files.Select(f => new UserFileDto
            {
                Id = f.Id,
                FileName = f.FileName,
                FileSizeBytes = f.FileSizeBytes,
                FileSizeFormatted = FormatFileSize(f.FileSizeBytes),
                MimeType = f.MimeType,
                UploadedAt = f.UploadedAt,
                CanBePrinted = FileSettings.AllowedExtensions.Contains(Path.GetExtension(f.FileName).ToLowerInvariant())
            }).ToList();

            return Results.Ok(ApiResponseDto<List<UserFileDto>>.SuccessResult(fileDtos));
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при извлечении файлов"
            });
        }
    }

    private static async Task<IResult> GetFileById(
        Guid id, 
        HttpContext context,
        IUserFileRepository fileRepository)
    {
        try
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var file = await fileRepository.GetByIdAsync(id);
            if (file == null || file.UserId != userId)
            {
                return Results.NotFound(new ErrorResponseDto
                {
                    Message = "Файл не найден"
                });
            }

            var fileDto = new UserFileDto
            {
                Id = file.Id,
                FileName = file.FileName,
                FileSizeBytes = file.FileSizeBytes,
                FileSizeFormatted = FormatFileSize(file.FileSizeBytes),
                MimeType = file.MimeType,
                UploadedAt = file.UploadedAt,
                CanBePrinted = FileSettings.AllowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant())
            };

            return Results.Ok(ApiResponseDto<UserFileDto>.SuccessResult(fileDto));
        }
        catch (Exception e)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при извлечении файла"
            });
        }
    }


    private static async Task<IResult> DeleteFile(
        Guid id, 
        HttpContext context, 
        IUserFileRepository fileRepository)
    {
        try
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var file = await fileRepository.GetByIdAsync(id);
            if (file == null || file.UserId != userId)
            {
                return Results.NotFound(new ErrorResponseDto
                {
                    Message = "Файл не найден"
                });
            }
            
            await fileRepository.SoftDeleteAsync(id);

            return Results.Ok(ApiResponseDto<bool>.SuccessResult(true, "Файл удален успешно"));
        }
        catch (Exception e)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при удалении файла"
            });
        }
    }
    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        var order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}