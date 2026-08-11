using Microsoft.EntityFrameworkCore;
using Sample.Domain;

namespace Sample.Persistence;

internal sealed class UserRepository(IDbContextFactory<UserDbContext> factory) : IUserDataAccess
{
    public async Task<User?> FindByIdAsync(string userId, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();

        return await db.Users
            .Where(u => u.UserId == userId)
            .Select(u => new User(u.UserId, u.Password, u.UserName))
            .FirstAsync(ct);
    }
}
