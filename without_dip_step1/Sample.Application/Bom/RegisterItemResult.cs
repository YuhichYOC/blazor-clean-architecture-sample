namespace Sample.Application.Registration;

/// <summary>
/// 登録ユースケースの結果。
///
/// 業務エラー(不変条件違反)は例外のまま UI へ飛ばさず、結果型として返す。
/// = DomainException を「想定内の業務エラー(400 相当)」としてユースケースが受け止める。
///   (DbException 等の“想定外”エラーは握りつぶさず伝播させ、境界で 500 相当に扱う。)
///
/// パターンマッチで分岐できるよう、閉じた継承(sealed record 派生)にしている。
/// </summary>
public abstract record RegisterItemResult
{
    private RegisterItemResult() { }

    /// <summary>登録成功。</summary>
    public sealed record Success : RegisterItemResult;

    /// <summary>
    /// 不変条件違反。Message はそのまま画面に出せる業務エラー文言
    /// (例: 「品番は必須です。」「所要量は0より大きい必要があります…」)。
    /// </summary>
    public sealed record ValidationError(string Message) : RegisterItemResult;
}
