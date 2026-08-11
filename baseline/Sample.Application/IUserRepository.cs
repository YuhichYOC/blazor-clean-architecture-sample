using Sample.Domain;

namespace Sample.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> FindByIdAsync(string userId, CancellationToken ct = default);
}
