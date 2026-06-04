using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Specifications;

public class AllReportsSpecification : BaseSpecification<ScoutReport>
{
    public AllReportsSpecification(Guid? playerId = null)
        : base(report => playerId == null || report.PlayerId == playerId)
    {
        AddInclude(report => report.Player);
        AddInclude(report => report.Scout);
        ApplyOrderByDescending(report => report.CreatedAt);
    }
}
