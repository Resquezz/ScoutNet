using ScoutNet.Domain.Entities;
using ScoutNet.Domain.Specifications;

namespace ScoutNet.Application.Specifications;

public class ReportByIdWithDetailsSpecification : BaseSpecification<ScoutReport>
{
    public ReportByIdWithDetailsSpecification(Guid id)
        : base(report => report.Id == id)
    {
        AddInclude(report => report.Player);
        AddInclude(report => report.Scout);
    }
}
