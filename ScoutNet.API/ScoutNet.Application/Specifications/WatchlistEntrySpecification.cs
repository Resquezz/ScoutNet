using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Specifications;

public class WatchlistEntrySpecification : BaseSpecification<Watchlist>
{
    public WatchlistEntrySpecification(Guid scoutId, Guid playerId)
        : base(entry => entry.ScoutId == scoutId && entry.PlayerId == playerId)
    {
    }
}
