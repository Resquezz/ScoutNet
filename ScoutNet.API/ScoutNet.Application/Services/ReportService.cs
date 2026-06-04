using FluentValidation;
using ScoutNet.Application.DTOs;
using ScoutNet.Application.Interfaces;
using ScoutNet.Application.Interfaces.Repositories;
using ScoutNet.Application.Interfaces.Services;
using ScoutNet.Application.Specifications;
using ScoutNet.Domain.Entities;

namespace ScoutNet.Application.Services;

public class ReportService(
    IReportRepository reportRepository,
    IPlayerRepository playerRepository,
    IValidator<CreateReportDto> createReportValidator,
    IUnitOfWork unitOfWork) : IReportService
{
    public async Task<ScoutReportDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var report = await reportRepository.GetByIdAsync(id, cancellationToken);
        return report is null ? null : ToDto(report);
    }

    public async Task<ScoutReportDto> CreateAsync(
        CreateReportDto dto,
        Guid scoutId,
        CancellationToken cancellationToken = default)
    {
        await createReportValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var player = await playerRepository.GetBySpecAsync(
            new PlayerByExternalIdWithStatisticsSpecification(dto.PlayerId),
            cancellationToken);

        if (player is null)
        {
            throw new KeyNotFoundException($"Player with id '{dto.PlayerId}' was not found.");
        }

        var report = new ScoutReport
        {
            Id = Guid.NewGuid(),
            ScoutId = scoutId,
            PlayerId = player.Id,
            CurrentForm = dto.CurrentForm,
            Potential = dto.Potential,
            Pros = dto.Pros,
            Cons = dto.Cons,
            Summary = dto.Summary,
            CreatedAt = DateTime.UtcNow,
        };

        await reportRepository.AddAsync(report, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(report);
    }

    public async Task DeleteAsync(Guid id, Guid scoutId, CancellationToken cancellationToken = default)
    {
        var report = await reportRepository.GetByIdAsync(id, cancellationToken);
        if (report is null)
        {
            throw new KeyNotFoundException($"Report with id '{id}' was not found.");
        }

        if (report.ScoutId != scoutId)
        {
            throw new UnauthorizedAccessException("You can only delete your own reports.");
        }

        reportRepository.Remove(report);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static ScoutReportDto ToDto(ScoutReport report) => new()
    {
        Id = report.Id,
        ScoutId = report.ScoutId,
        PlayerId = report.PlayerId,
        CurrentForm = report.CurrentForm,
        Potential = report.Potential,
        Pros = report.Pros,
        Cons = report.Cons,
        Summary = report.Summary,
        CreatedAt = report.CreatedAt,
    };
}
