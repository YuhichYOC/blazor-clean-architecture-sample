using Sample.Application.Abstractions;
using Sample.Domain;

namespace Sample.Application.UseCases;

public sealed class AuthenticateUser
{
    private readonly IUserRepository _users;
    public AuthenticateUser(IUserRepository users) => _users = users;

    public async Task<User?> ExecuteAsync(string userId, string password, CancellationToken ct = default)
    {
        var user = await _users.FindByIdAsync(userId, ct);
        if (user is null) return null;
        return user.VerifyPassword(password) ? user : null;
    }
}
