using ITPrint.API.Common;
using ITPrint.Core.DTOs.Common;
using ITPrint.Core.DTOs.Printers;
using ITPrint.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ITPrint.API.Endpoints;

public static class PrinterEndpoint
{
    public static IEndpointRouteBuilder MapPrinterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/printers")
            .WithTags("Printers")
            .WithOpenApi();
        
        group.MapGet("/", GetAllPrinters)
            .Produces<ApiResponseDto<List<PrinterDto>>>();
        
        group.MapGet("active", GetActivePrinters)
            .Produces<ApiResponseDto<List<PrinterDto>>>();
        
        group.MapGet("{printerId:guid}", GetPrinterById)
            .Produces<ApiResponseDto<PrinterWithCapabilitiesDto>>()
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapPost("/", CreatePrinter)
            .RequireAuthorization(options => 
                options.AddRequirements(requirements: new RolesAuthorizationRequirement(["Admin"])))
            .Validate<CreatePrinterDto>()
            .Produces<ApiResponseDto<PrinterDto>>(StatusCodes.Status201Created)
            .Produces<ErrorResponseDto>(StatusCodes.Status400BadRequest);
        
        group.MapPut("/", UpdatePrinter)
            .RequireAuthorization(options => 
                options.AddRequirements(requirements: new RolesAuthorizationRequirement(["Admin"])))
            .Validate<UpdatePrinterDto>()
            .Produces<ApiResponseDto<PrinterDto>>()
            .Produces<ErrorResponseDto>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("{printerId:guid}", DeletePrinter)
            .RequireAuthorization(options =>
                options.AddRequirements(requirements: new RolesAuthorizationRequirement(["Admin"])))
            .Produces<ApiResponseDto<bool>>()
            .Produces(StatusCodes.Status404NotFound);
        
        group.MapPost("{id:guid}/capabilities", AddCapability)
            .RequireAuthorization(options =>
                options.AddRequirements(requirements: new RolesAuthorizationRequirement(["Admin"])))
            .Validate<AddCapabilityDto>()
            .Produces<ApiResponseDto<PrinterCapabilityDto>>(StatusCodes.Status201Created)
            .Produces<ErrorResponseDto>(StatusCodes.Status400BadRequest);
        
        return group;
    }

    private static async Task<IResult> GetAllPrinters(IPrinterService printerService)
    {
        try
        {
            var printers = await printerService.GetAllPrintersAsync();
            var printerDtos = printers.Select(p => new PrinterDto
            {
                Id = p.Id,
                Name = p.Name,
                Model = p.Model,
                CupsName = p.CupsName,
                Location = p.Location,
                Description = p.Description,
                Status = p.Status,
                Priority = p.Priority,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                LastPrintedAt = p.LastPrintedAt
            }).ToList();

            return Results.Ok(ApiResponseDto<List<PrinterDto>>.SuccessResult(printerDtos));
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при извлечении принтеров"
            });
        }
    }

    private static async Task<IResult> GetActivePrinters(IPrinterService printerService)
    {
        try
        {
            var printers = await printerService.GetActivePrintersAsync();
            var printerDtos = printers.Select(p => new PrinterDto
            {
                Id = p.Id,
                Name = p.Name,
                Model = p.Model,
                CupsName = p.CupsName,
                Location = p.Location,
                Description = p.Description,
                Status = p.Status,
                Priority = p.Priority,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                LastPrintedAt = p.LastPrintedAt
            }).ToList();

            return Results.Ok(ApiResponseDto<List<PrinterDto>>.SuccessResult(printerDtos));
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при извлечении принтеров"
            });
        }
    }

    private static async Task<IResult> GetPrinterById(
        Guid printerId,
        IPrinterService printerService)
    {
        try
        {
            var printer = await printerService.GetPrinterWithCapabilitiesAsync(printerId);
            if (printer == null)
            {
                return Results.NotFound(new ErrorResponseDto
                {
                    Message = $"Принтер с идентификатором {printerId} не найден"
                });
            }

            var printerDto = new PrinterWithCapabilitiesDto
            {
                Id = printer.Id,
                Name = printer.Name,
                Model = printer.Model,
                CupsName = printer.CupsName,
                Location = printer.Location,
                Description = printer.Description,
                Status = printer.Status,
                Priority = printer.Priority,
                IsActive = printer.IsActive,
                CreatedAt = printer.CreatedAt,
                LastPrintedAt = printer.LastPrintedAt,
                Capabilities = printer.Capabilities.Select(c => new PrinterCapabilityDto
                {
                    Id = c.Id,
                    PaperFormat = c.PaperFormat,
                    ColorSupport = c.ColorSupport,
                    DuplexSupport = c.DuplexSupport,
                    MaxCopies = c.MaxCopies,
                    MaxPagesPerJob = c.MaxPagesPerJob
                }).ToList(),
                CurrentQueueSize = printer.QueueItems.Count
            };

            return Results.Ok(ApiResponseDto<PrinterWithCapabilitiesDto>.SuccessResult(printerDto));
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при извлечении принтера"
            });
        }
    }

    private static async Task<IResult> CreatePrinter(
        IPrinterService  printerService,
        [FromBody] CreatePrinterDto request)
    {
        try
        {
            var printer = await printerService.CreatePrinterAsync(
                request.Name,
                request.Model,
                request.CupsName,
                request.Location,
                request.Description,
                request.Priority
            );

            var printerDto = new PrinterDto
            {
                Id = printer.Id,
                Name = printer.Name,
                Model = printer.Model,
                CupsName = printer.CupsName,
                Location = printer.Location,
                Description = printer.Description,
                Status = printer.Status,
                Priority = printer.Priority,
                IsActive = printer.IsActive,
                CreatedAt = printer.CreatedAt
            };

            return Results.Created(printer.Id.ToString(),
                ApiResponseDto<PrinterDto>.SuccessResult(printerDto, "Принтер создан успешно")
            );
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponseDto
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при создании принтера"
            });
        }
    }

    private static async Task<IResult> UpdatePrinter(
        Guid printerId,
        [FromBody] UpdatePrinterDto request,
        IPrinterService printerService)
    {
        try
        {
            var printer = await printerService.UpdatePrinterAsync(
                printerId,
                request.Name,
                request.Model,
                request.Location,
                request.Description,
                request.Priority,
                request.Status,
                request.IsActive
            );

            var printerDto = new PrinterDto
            {
                Id = printer.Id,
                Name = printer.Name,
                Model = printer.Model,
                CupsName = printer.CupsName,
                Location = printer.Location,
                Description = printer.Description,
                Status = printer.Status,
                Priority = printer.Priority,
                IsActive = printer.IsActive,
                CreatedAt = printer.CreatedAt,
                LastPrintedAt = printer.LastPrintedAt
            };

            return Results.Ok(ApiResponseDto<PrinterDto>.SuccessResult(printerDto, "Принтер успешно обновлен"));
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new ErrorResponseDto
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при обновлении принтера"
            });
        }
    }

    private static async Task<IResult> DeletePrinter(
        Guid printerId,
        IPrinterService printerService)
    {
        try
        {
            await printerService.DeletePrinterAsync(printerId);
            return Results.Ok(ApiResponseDto<bool>.SuccessResult(true, "Принтер успешно удален"));
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new ErrorResponseDto
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при удалении принтера"
            });
        }
    }

    private static async Task<IResult> AddCapability(
        Guid id, 
        [FromBody] AddCapabilityDto request,
        IPrinterService printerService)
    {
        try
        {
            var capability = await printerService.AddCapabilityAsync(
                id,
                request.PaperFormat,
                request.ColorSupport,
                request.DuplexSupport,
                request.MaxCopies,
                request.MaxPagesPerJob
            );

            var capabilityDto = new PrinterCapabilityDto
            {
                Id = capability.Id,
                PaperFormat = capability.PaperFormat,
                ColorSupport = capability.ColorSupport,
                DuplexSupport = capability.DuplexSupport,
                MaxCopies = capability.MaxCopies,
                MaxPagesPerJob = capability.MaxPagesPerJob
            };

            return Results.Created(id.ToString(),
                ApiResponseDto<PrinterCapabilityDto>.SuccessResult(capabilityDto, "Возможности принтера успешно добавлены")
            );
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new ErrorResponseDto
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponseDto
            {
                Message = "Произошла ошибка при добавлении возможности"
            });
        }
    }
}

