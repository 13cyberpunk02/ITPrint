using ITPrint.Core.Enums;
using ITPrint.Core.Interfaces.Repositories;
using ITPrint.Core.Interfaces.Services;
using ITPrint.Core.Models;
using Microsoft.Extensions.Logging;

namespace ITPrint.Application.Services;


public class PrinterService(
    IPrinterRepository printerRepository,
    ILogger<PrinterService> logger)
    : IPrinterService
{
    public async  Task<IEnumerable<Printer>> GetAllPrintersAsync(CancellationToken cancellationToken = default)
        => await printerRepository.GetAllAsync(cancellationToken);

    public async Task<IEnumerable<Printer>> GetActivePrintersAsync(CancellationToken cancellationToken = default)
        => await printerRepository.GetActivePrintersAsync(cancellationToken);

    public async Task<IEnumerable<Printer>> GetPrintersByFormatAsync(PaperFormat format, CancellationToken cancellationToken = default)
        => await printerRepository.GetPrintersByFormatAsync(format, cancellationToken);

    public async Task<Printer?> GetPrinterByIdAsync(Guid printerId, CancellationToken cancellationToken = default)
        => await printerRepository.GetByIdAsync(printerId, cancellationToken);

    public async Task<Printer?> GetPrinterWithCapabilitiesAsync(Guid printerId, CancellationToken cancellationToken = default)
        => await printerRepository.GetPrinterWithCapabilitiesAsync(printerId, cancellationToken);

    public async Task<Printer> CreatePrinterAsync(string name, string model, string cupsName, string location, string description, int priority,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = await printerRepository.GetByCupsNameAsync(cupsName, cancellationToken);
            if (existing != null)
            {
                throw new InvalidOperationException($"Принтер с именем в CUPS сервере {cupsName} уже существует");
            }

            var printer = new Printer
            {
                Id = Guid.NewGuid(),
                Name = name,
                Model = model,
                CupsName = cupsName,
                Location = location,
                Description = description,
                Status = PrinterStatus.Active,
                Priority = priority,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await printerRepository.AddAsync(printer, cancellationToken);

            logger.LogInformation("Создан принтер: {PrinterName} в CUPS сервере ({CupsName})", name, cupsName);

            return printer;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка создания принтера {PrinterName}", name);
            throw;
        }
    }

    public async Task<Printer> UpdatePrinterAsync(Guid printerId, string? name, string? model, string? location, string? description,
        int? priority, PrinterStatus? status, bool? isActive, CancellationToken cancellationToken = default)
    {
        try
        {
            var printer = await printerRepository.GetByIdAsync(printerId, cancellationToken);
            if (printer == null)
            {
                throw new InvalidOperationException($"Принтер с идентификатором {printerId} не найден");
            }

            if (!string.IsNullOrEmpty(name))
                printer.Name = name;

            if (!string.IsNullOrEmpty(model))
                printer.Model = model;

            if (!string.IsNullOrEmpty(location))
                printer.Location = location;

            if (!string.IsNullOrEmpty(description))
                printer.Description = description;

            if (priority.HasValue)
                printer.Priority = priority.Value;

            if (status.HasValue)
                printer.Status = status.Value;

            if (isActive.HasValue)
                printer.IsActive = isActive.Value;

            await printerRepository.UpdateAsync(printer, cancellationToken);

            logger.LogInformation("Принтер обновлен: {PrinterId}", printerId);

            return printer;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка обновления принтера {PrinterId}", printerId);
            throw;
        }
    }

    public async Task DeletePrinterAsync(Guid printerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var printer = await printerRepository.GetByIdAsync(printerId, cancellationToken);
            if (printer == null)
            {
                throw new InvalidOperationException($"Принтер с идентификатором {printerId} не найден");
            }

            // Soft delete by deactivating
            printer.IsActive = false;
            printer.Status = PrinterStatus.Inactive;
            await printerRepository.UpdateAsync(printer, cancellationToken);

            logger.LogInformation("Принтер деактивирован: {PrinterId}", printerId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка удаления принтера {PrinterId}", printerId);
            throw;
        }
    }

    public async Task<PrinterCapability> AddCapabilityAsync(Guid printerId, PaperFormat format, bool colorSupport, bool duplexSupport, int maxCopies,
        int maxPagesPerJob, CancellationToken cancellationToken = default)
    {
        try
        {
            var printer = await printerRepository.GetPrinterWithCapabilitiesAsync(printerId, cancellationToken);
            if (printer == null)
            {
                throw new InvalidOperationException($"Принтер с идентификатором {printerId} не найден");
            }

            // Check if capability already exists
            if (printer.Capabilities.Any(c => c.PaperFormat == format))
            {
                throw new InvalidOperationException($"Принтер уже поддерживает формат {format}");
            }

            var capability = new PrinterCapability
            {
                Id = Guid.NewGuid(),
                PrinterId = printerId,
                PaperFormat = format,
                ColorSupport = colorSupport,
                DuplexSupport = duplexSupport,
                MaxCopies = maxCopies,
                MaxPagesPerJob = maxPagesPerJob
            };

            printer.Capabilities.Add(capability);
            await printerRepository.UpdateAsync(printer, cancellationToken);

            logger.LogInformation("Возможность печати формата {Format} добавлена к принтеру {PrinterId}", format, printerId);

            return capability;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка добавления возможности печати формата {Format} принтеру {PrinterId}", format, printerId);
            throw;
        }
    }

    public Task RemoveCapabilityAsync(Guid capabilityId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Для этого потребуется еще один репозиторий PrinterCapabilityRepository
            // Сейчас убиваем поток и выдаем исключение NotImplementedException
            throw new NotImplementedException("Для данного метода RemoveCapabilityAsync нужно будет создать PrinterCapabilityRepository");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка удаления возможности {CapabilityId}", capabilityId);
            throw;
        }
    }

    public async Task UpdatePrinterStatusAsync(Guid printerId, PrinterStatus status, CancellationToken cancellationToken = default)
    {
        await printerRepository.UpdateStatusAsync(printerId, status, cancellationToken);
        logger.LogInformation("Printer {PrinterId} status updated to {Status}", printerId, status);
    }
}