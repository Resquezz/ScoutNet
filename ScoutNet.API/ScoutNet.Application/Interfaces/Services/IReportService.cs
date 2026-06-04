using ScoutNet.Application.DTOs;

namespace ScoutNet.Application.Interfaces.Services;

public interface IReportService
{
    Task<ScoutReportDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ScoutReportDto> CreateAsync(
        CreateReportDto dto,
        Guid scoutId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, Guid scoutId, CancellationToken cancellationToken = default);
}
