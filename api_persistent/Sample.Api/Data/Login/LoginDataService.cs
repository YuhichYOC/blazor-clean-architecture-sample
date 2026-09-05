using Sample.Api.Contracts.Login;
using Sample.Api.Entities;
using Microsoft.EntityFrameworkCore;

/*
 * このクラスの内容は baseline での UserRepository と同じ
 */
namespace Sample.Api.Data.Login;

public sealed class LoginDataService(IDbContextFactory<UserDbContext> factory)
{
    public async Task<UserResponse?> FindByIdAsync(string userId, CancellationToken ct = default)
    {
        await using var db = factory.CreateDbContext();

        return await db.Users
            .Where(u => u.UserId == userId)
            .Select(u => new UserResponse(u.UserId, u.UserName, u.Password))
            .FirstOrDefaultAsync(ct); // userId = USERS.USER_ID でヒットする行が存在しないときに null を返す, FirstAsync ではヒットする行が存在しないときに実行時例外になる
    }
}
