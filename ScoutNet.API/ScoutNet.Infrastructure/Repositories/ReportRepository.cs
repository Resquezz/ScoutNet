using Microsoft.EntityFrameworkCore;
using ScoutNet.Application.Interfaces.Repositories;
using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;
using ScoutNet.Infrastructure.Persistence;
using ScoutNet.Infrastructure.Specifications;

namespace ScoutNet.Infrastructure.Repositories;

public class ReportRepository(ScoutDbContext dbContext) : IReportRepository
{
    public Task<ScoutReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.ScoutReports.FirstOrDefaultAsync(report => report.Id == id, cancellationToken);

    public Task<ScoutReport?> GetBySpecAsync(
        ISpecification<ScoutReport> spec,
        CancellationToken cancellationToken = default) =>
        SpecificationEvaluator
            .GetQuery(dbContext.ScoutReports.AsQueryable(), spec)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<ScoutReport>> ListBySpecAsync(
        ISpecification<ScoutReport> spec,
        CancellationToken cancellationToken = default) =>
        await SpecificationEvaluator
            .GetQuery(dbContext.ScoutReports.AsQueryable(), spec)
            .ToListAsync(cancellationToken);

    public Task<int> CountBySpecAsync(
        ISpecification<ScoutReport> spec,
        CancellationToken cancellationToken = default) =>
        SpecificationEvaluator
            .GetQuery(dbContext.ScoutReports.AsQueryable(), spec)
            .CountAsync(cancellationToken);

    public async Task AddAsync(ScoutReport report, CancellationToken cancellationToken = default) =>
        await dbContext.ScoutReports.AddAsync(report, cancellationToken);

    public void Update(ScoutReport report) => dbContext.ScoutReports.Update(report);

    public void Remove(ScoutReport report) => dbContext.ScoutReports.Remove(report);
}
