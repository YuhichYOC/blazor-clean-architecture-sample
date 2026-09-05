namespace Sample.Application.Abstractions;
 
/// <summary>
/// 「書き込みポート」が報告できる結果。
///
/// 永続化境界が実際に区別できるのは Success か Conflict(一意制約違反など)だけ。
/// ドメイン検証(空品番・所要量0以下など)は、この境界に来る前にユースケースが
/// DomainException として弾いているため、ここには現れない。
/// だからこの型に ValidationError は無い(在処が違う)。
///
/// RegisterItemResult(ユースケース結果)とは別物である点が要点:
///   WriteOutcome        = 永続化“境界”が知り得ること (Success / Conflict)
///   RegisterItemResult  = ユースケース“全体”の結末   (Success / ValidationError / Conflict)
/// ユースケースが両者を突き合わせて後者を組み立てる。
/// </summary>
public abstract record WriteOutcome
{
    private WriteOutcome() { }
 
    /// <summary>書き込み成功。</summary>
    public sealed record Success : WriteOutcome;
 
    /// <summary>
    /// 一意制約違反など、想定内の業務衝突。
    /// Message はそのまま画面に出せる文言(例:「同じ品番が既に登録されています。」)。
    /// </summary>
    public sealed record Conflict(string Message) : WriteOutcome;
}
