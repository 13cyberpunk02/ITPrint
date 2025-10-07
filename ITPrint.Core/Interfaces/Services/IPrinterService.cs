using ITPrint.Core.Enums;
using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Services;

public interface IPrinterService
{
    Task<IEnumerable<Printer>> GetAllPrintersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Printer>> GetActivePrintersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Printer>> GetPrintersByFormatAsync(PaperFormat format, CancellationToken cancellationToken = default);
    Task<Printer?> GetPrinterByIdAsync(Guid printerId, CancellationToken cancellationToken = default);
    Task<Printer?> GetPrinterWithCapabilitiesAsync(Guid printerId, CancellationToken cancellationToken = default);
    Task<Printer> CreatePrinterAsync(string name, string model, string cupsName, string location, string description, int priority, CancellationToken cancellationToken = default);
    Task<Printer> UpdatePrinterAsync(Guid printerId, string? name, string? model, string? location, string? description, int? priority, PrinterStatus? status, bool? isActive, CancellationToken cancellationToken = default);
    Task DeletePrinterAsync(Guid printerId, CancellationToken cancellationToken = default);
    Task<PrinterCapability> AddCapabilityAsync(Guid printerId, PaperFormat format, bool colorSupport, bool duplexSupport, int maxCopies, int maxPagesPerJob, CancellationToken cancellationToken = default);
    Task RemoveCapabilityAsync(Guid capabilityId, CancellationToken cancellationToken = default);
    Task UpdatePrinterStatusAsync(Guid printerId, PrinterStatus status, CancellationToken cancellationToken = default);
}