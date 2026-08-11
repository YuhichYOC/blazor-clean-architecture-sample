using Microsoft.EntityFrameworkCore;
using Sample.Application.Abstractions;
using Sample.Domain;

namespace Sample.Persistence;

internal sealed class UserRepository(IDbContextFactory<BomDbContext> factory) : IUserRepository
{
    public async Task<User?> FindByIdAsync(string userId, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();

        return await db.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId, ct);
    }
}
