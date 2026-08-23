namespace Sample.Domain;

/// <summary>
/// ドメインの不変条件に違反したときに投げる例外。
/// インフラ由来の例外(DbException 等)と区別するための専用型にしておくと、
/// アプリケーション層で「業務エラー(400 相当)」と「システムエラー(500 相当)」を切り分けやすい。
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
