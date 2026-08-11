using Sample.Domain;

namespace Sample.Persistence;

public interface IUserDataAccess
{
    Task<User?> FindByIdAsync(string userId, CancellationToken ct = default);
}
