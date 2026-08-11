using Sample.Domain;
using Sample.Application.Abstractions;

namespace Sample.Persistence;

public sealed class UserRepositoryAdapter(IUserDataAccess inner) : IUserRepository
{
    public async Task<User?> FindByIdAsync(string userId, CancellationToken ct = default)
        => await inner.FindByIdAsync(userId, ct);
}
