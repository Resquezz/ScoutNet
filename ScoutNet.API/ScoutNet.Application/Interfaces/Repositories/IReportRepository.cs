using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Interfaces.Repositories;

public interface IReportRepository
{
    Task<ScoutReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ScoutReport?> GetBySpecAsync(ISpecification<ScoutReport> spec, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ScoutReport>> ListBySpecAsync(ISpecification<ScoutReport> spec, CancellationToken cancellationToken = default);

    Task<int> CountBySpecAsync(ISpecification<ScoutReport> spec, CancellationToken cancellationToken = default);

    Task AddAsync(ScoutReport report, CancellationToken cancellationToken = default);

    void Update(ScoutReport report);

    void Remove(ScoutReport report);
}
