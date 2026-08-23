using Sample.Domain;
using Sample.Persistence;   // IUserDataAccess(低レベル側が所有する抽象)

namespace Sample.Application.UseCases;

/*
 * ログイン機能のユースケース(ログインボタン → 成功/失敗までのひと塊)。
 *
 * ── ステップ1(DIP の反転除去)による変更点 ────────────────────────────────
 *   before: AuthenticateUseCase(IUserRepository users)  … Application 所有のポートに依存
 *   after : AuthenticateUseCase(IUserDataAccess users)  … Persistence 所有の抽象に依存
 *
 *   User 側は BOM 側と違い、境界での変換が無い(戻り値は両側ともドメインの User)。
 *   そのため UserRepositoryAdapter は元々ただの素通しで、ベースラインのコメントでも
 *   「実はアダプタを挟む必要がない」と注記されていた。
 *   反転を外すと、その素通しアダプタは丸ごと消えるだけで、ここのロジックは一切変わらない。
 *   → 「アダプタが変換の仕事を持たない境界では、DIP の反転はほぼ純粋なコストだった」ことが露わになる。
 *
 *   なお users の宣言型は IUserDataAccess(public インターフェース)なので、実装の
 *   UserRepository が internal のままでも問題ない(DI が生成し、インターフェース経由で注入する)。
 *   ステップ2で IUserDataAccess も外して具象 UserRepository を直接注入する場合は、
 *   その時点で UserRepository を public へ引き上げる必要が出る(実装隠蔽が失われる = さらなる代償)。
 */
public sealed class AuthenticateUseCase
{
    private readonly IUserDataAccess _users;
    public AuthenticateUseCase(IUserDataAccess users) => _users = users;

    // ログイン処理
    //   成功 : レコードあり & パスワード一致 → User インスタンスを返す
    //   失敗 : レコードなし or パスワード不一致 → null を返す
    public async Task<User?> ExecuteAsync(string userId, string password, CancellationToken ct = default)
    {
        var user = await _users.FindByIdAsync(userId, ct);
        if (user is null) return null;                       // ユーザーIDヒットなし
        return user.VerifyPassword(password) ? user : null;  // パスワード検証
    }
}
