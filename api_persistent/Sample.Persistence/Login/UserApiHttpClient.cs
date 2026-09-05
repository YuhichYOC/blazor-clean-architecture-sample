using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Sample.Domain;   // ドメイン集約 User
 
namespace Sample.Persistence;
 
/// <summary>
/// 差し替え版 <see cref="IUserDataAccess"/> 実装(案A)。
///
/// GET /api/users/{userId} を呼び、UserResponse(JSON) から ドメイン <see cref="User"/> を復元して返す。
/// baseline の UserRepository(EF)が担っていた「行 → ドメイン User」の復元を、
/// 「JSON → ドメイン User」に置き換えたもの。上位(UserRepositoryAdapter・AuthenticateUseCase)は無変更。
///
/// 押さえる点3つ:
///   1. 404 は「ユーザー不在」という“想定内の結果” → null を返す(例外にしない)。
///      それ以外の非成功(5xx・ネットワーク等)は“想定外” → 例外で境界へ。
///      ※ baseline の FirstOrDefaultAsync が「ヒット無し=null」だったのと同じ意味を HTTP で再現する。
///   2. 受信 DTO からドメイン User を再構築する際、User のコンストラクタが不変条件を再検査する。
///      API 由来のデータであってもドメインが門番になる(空 userId/userName を弾く)。
///   3. ★要確認(案Aの既知のコスト): パスワードが毎回ワイヤを流れる。TLS 必須。
///      本サンプルは平文前提であることも承知の上で使うこと。
/// </summary>
public sealed class UserApiHttpClient(HttpClient http) : IUserDataAccess
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
 
    public async Task<User?> FindByIdAsync(string userId, CancellationToken ct = default)
    {
        // GetFromJsonAsync は非成功ステータスで例外を投げる(内部で EnsureSuccessStatusCode)。
        // 404 を null に落としたいので、ここでは使わず GetAsync で自前判定する。
        using var res = await http.GetAsync(
            $"api/login/{Uri.EscapeDataString(userId)}", ct);
 
        // 想定内: ユーザー不在。呼び出し側(AuthenticateUseCase)は null を認証失敗として扱う。
        if (res.StatusCode == HttpStatusCode.NotFound)
            return null;
 
        // 想定外(5xx・ネットワーク等)はここで例外化して境界へ伝播させる。
        res.EnsureSuccessStatusCode();
 
        var dto = await res.Content.ReadFromJsonAsync<UserDto>(Json, ct);
        if (dto is null)
            return null;   // 200 だが本文が空/null。念のため。
 
        // JSON → ドメイン集約へ復元。ここでコンストラクタが不変条件を再検査する。
        // ★注意: User(userId, password, userName) の並び。password が第2引数(取り違えやすい)。
        return new User(dto.UserId, dto.Password, dto.UserName);
    }
 
    /// <summary>
    /// 受信専用のワイヤ DTO。サーバーの UserResponse と構造一致していればよい。
    /// baseline の User 側にはパーシステンス DTO が無かったため、これは HTTP 化で新たに要る産物。
    /// (Bom 側の BomRowDto に相当する位置づけを、User では初めて用意することになる)
    /// </summary>
    private sealed record UserDto(string UserId, string UserName, string Password);
}
