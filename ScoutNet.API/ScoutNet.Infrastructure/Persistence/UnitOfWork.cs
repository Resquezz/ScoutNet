using ScoutNet.Application.Interfaces;

namespace ScoutNet.Infrastructure.Persistence;

public class UnitOfWork(ScoutDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
