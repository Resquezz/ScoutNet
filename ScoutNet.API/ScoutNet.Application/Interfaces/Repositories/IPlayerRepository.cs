using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Interfaces.Repositories;

public interface IPlayerRepository
{
    Task<Player?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Player?> GetBySpecAsync(ISpecification<Player> spec, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Player>> ListBySpecAsync(ISpecification<Player> spec, CancellationToken cancellationToken = default);

    Task<int> CountBySpecAsync(ISpecification<Player> spec, CancellationToken cancellationToken = default);

    Task AddAsync(Player player, CancellationToken cancellationToken = default);

    void Update(Player player);

    void Remove(Player player);
}
