using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Specifications;

public class PlayerByExternalIdWithStatisticsSpecification : BaseSpecification<Player>
{
    public PlayerByExternalIdWithStatisticsSpecification(int externalId)
        : base(player => player.ExternalId == externalId)
    {
        AddInclude(player => player.TeamProfile);
        AddInclude(player => player.LeagueProfile);
        AddInclude(player => player.Statistics);
        AddIncludeChain("Statistics.Team");
        AddIncludeChain("Statistics.League");
    }
}
