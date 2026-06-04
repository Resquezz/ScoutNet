using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Specifications;

public class ReportsByScoutSpecification : BaseSpecification<ScoutReport>
{
    public ReportsByScoutSpecification(Guid scoutId, Guid? playerId = null)
        : base(report => report.ScoutId == scoutId && (playerId == null || report.PlayerId == playerId))
    {
        AddInclude(report => report.Player);
        ApplyOrderByDescending(report => report.CreatedAt);
    }
}
