using FluentValidation;
using ScoutNet.Application.DTOs;
using ScoutNet.Application.Interfaces;
using ScoutNet.Application.Interfaces.Repositories;
using ScoutNet.Application.Interfaces.Services;
using ScoutNet.Application.Specifications;
using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Enums;

namespace ScoutNet.Application.Services;

public class ReportService(
    IReportRepository reportRepository,
    IPlayerRepository playerRepository,
    IValidator<CreateReportDto> createReportValidator,
    IValidator<UpdateReportDto> updateReportValidator,
    IUnitOfWork unitOfWork) : IReportService
{
    public async Task<ScoutReportDto?> GetByIdAsync(
        Guid id,
        Guid userId,
        UserRole userRole,
        CancellationToken cancellationToken = default)
    {
        var report = await reportRepository.GetBySpecAsync(
            new ReportByIdWithDetailsSpecification(id),
            cancellationToken);

        if (report is null)
        {
            return null;
        }

        if (userRole != UserRole.Admin && report.ScoutId != userId)
        {
            throw new UnauthorizedAccessException("You can only view your own reports.");
        }

        return ToDto(report);
    }

    public async Task<IReadOnlyList<ScoutReportDto>> GetReportsAsync(
        Guid userId,
        UserRole userRole,
        int? playerExternalId = null,
        CancellationToken cancellationToken = default)
    {
        Guid? internalPlayerId = null;
        if (playerExternalId.HasValue)
        {
            var player = await playerRepository.GetBySpecAsync(
                new PlayerByExternalIdWithStatisticsSpecification(playerExternalId.Value),
                cancellationToken);

            if (player is null)
            {
                throw new KeyNotFoundException($"Player with id '{playerExternalId}' was not found.");
            }

            internalPlayerId = player.Id;
        }

        var reports = userRole == UserRole.Admin
            ? await reportRepository.ListBySpecAsync(
                new AllReportsSpecification(internalPlayerId),
                cancellationToken)
            : await reportRepository.ListBySpecAsync(
                new ReportsByScoutSpecification(userId, internalPlayerId),
                cancellationToken);

        return reports.Select(ToDto).ToList();
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

        report.Player = player;
        return ToDto(report);
    }

    public async Task<ScoutReportDto> UpdateAsync(
        Guid id,
        UpdateReportDto dto,
        Guid scoutId,
        CancellationToken cancellationToken = default)
    {
        await updateReportValidator.ValidateAndThrowAsync(dto, cancellationToken);

        var report = await reportRepository.GetBySpecAsync(
            new ReportByIdWithDetailsSpecification(id),
            cancellationToken);

        if (report is null)
        {
            throw new KeyNotFoundException($"Report with id '{id}' was not found.");
        }

        if (report.ScoutId != scoutId)
        {
            throw new UnauthorizedAccessException("You can only edit your own reports.");
        }

        report.CurrentForm = dto.CurrentForm;
        report.Potential = dto.Potential;
        report.Pros = dto.Pros;
        report.Cons = dto.Cons;
        report.Summary = dto.Summary;

        reportRepository.Update(report);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToDto(report);
    }

    public async Task DeleteAsync(
        Guid id,
        Guid userId,
        UserRole userRole,
        CancellationToken cancellationToken = default)
    {
        var report = await reportRepository.GetByIdAsync(id, cancellationToken);
        if (report is null)
        {
            throw new KeyNotFoundException($"Report with id '{id}' was not found.");
        }

        if (userRole != UserRole.Admin && report.ScoutId != userId)
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
        PlayerExternalId = report.Player?.ExternalId ?? 0,
        PlayerName = report.Player?.Name ?? string.Empty,
        ScoutUsername = report.Scout?.Username ?? string.Empty,
        CurrentForm = report.CurrentForm,
        Potential = report.Potential,
        Pros = report.Pros,
        Cons = report.Cons,
        Summary = report.Summary,
        CreatedAt = report.CreatedAt,
    };
}
