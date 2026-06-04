using ScoutNet.Application.DTOs;
using ScoutNet.Domain.Enums;

namespace ScoutNet.Application.Interfaces.Services;

public interface IReportService
{
    Task<ScoutReportDto?> GetByIdAsync(
        Guid id,
        Guid userId,
        UserRole userRole,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ScoutReportDto>> GetReportsAsync(
        Guid userId,
        UserRole userRole,
        int? playerExternalId = null,
        CancellationToken cancellationToken = default);

    Task<ScoutReportDto> CreateAsync(
        CreateReportDto dto,
        Guid scoutId,
        CancellationToken cancellationToken = default);

    Task<ScoutReportDto> UpdateAsync(
        Guid id,
        UpdateReportDto dto,
        Guid scoutId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        Guid userId,
        UserRole userRole,
        CancellationToken cancellationToken = default);
}
