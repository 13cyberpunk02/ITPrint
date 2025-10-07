using ITPrint.Core.Enums;
using ITPrint.Core.Models;

namespace ITPrint.Core.Interfaces.Repositories;

public interface IPrinterRepository : IBaseRepository<Printer>
{
    Task<IEnumerable<Printer>> GetActivePrintersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Printer>> GetPrintersByFormatAsync(PaperFormat format, CancellationToken cancellationToken = default);
    Task<Printer?> GetPrinterWithCapabilitiesAsync(Guid printerId, CancellationToken cancellationToken = default);
    Task<Printer?> GetByCupsNameAsync(string cupsName, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid printerId, PrinterStatus status, CancellationToken cancellationToken = default);
}