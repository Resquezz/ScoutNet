using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Specifications;

public class WatchlistByScoutSpecification : BaseSpecification<Watchlist>
{
    public WatchlistByScoutSpecification(Guid scoutId)
        : base(entry => entry.ScoutId == scoutId)
    {
        AddInclude(entry => entry.Player);
        AddIncludeChain("Player.TeamProfile");
        AddIncludeChain("Player.LeagueProfile");
    }
}
