using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Specifications;

public class PlayerByExternalIdWithStatisticsSpecification : BaseSpecification<Player>
{
    public PlayerByExternalIdWithStatisticsSpecification(int externalId)
        : base(player => player.ExternalId == externalId)
    {
        AddInclude(player => player.Statistics);
    }
}
