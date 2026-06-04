using Microsoft.EntityFrameworkCore;
using ScoutNet.Application.Interfaces.Repositories;
using ScoutNet.Domain.Entities;
using ScoutNet.Infrastructure.Persistence;

namespace ScoutNet.Infrastructure.Repositories;

public class UserRepository(ScoutDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        dbContext.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        dbContext.Users.FirstOrDefaultAsync(user => user.Username == username, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await dbContext.Users.AddAsync(user, cancellationToken);
}
