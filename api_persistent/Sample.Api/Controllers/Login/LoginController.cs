using Sample.Api.Contracts.Login;
using Sample.Api.Data.Login;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Versioning;

namespace Sample.Api.Controllers.Login;

[ApiController]
[Route("api/login")]
public sealed class LoginController(LoginDataService data) : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(string userId, CancellationToken ct)
    {
        var dto = await data.FindByIdAsync(userId, ct);   // ↓ の UserDataService
        return dto is null ? NotFound() : Ok(dto);
    }

    // UserDataService（baseline UserRepository を移設。ただしドメインUserを作らず、EFエンティティから直接DTOへ）
    public async Task<UserResponse?> FindByIdAsync(string userId, CancellationToken ct = default)
    {
        return await data.FindByIdAsync(userId, ct);   // ★ドメインUserを経由しない
    }
}
