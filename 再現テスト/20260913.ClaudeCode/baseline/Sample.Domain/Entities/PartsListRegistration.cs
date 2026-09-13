namespace Sample.Domain.Entities;

/// <summary>
/// 部品構成表の新規登録内容（品番＋構成部品一覧）。
/// 登録時に Item・Material・Bom の3テーブルへ書き込む内容の元になる
/// （画面イメージ-追加ボタン・新規登録 参照）。
/// </summary>
/// <param name="ItemCode">品番</param>
/// <param name="ItemName">品名</param>
/// <param name="ConstituentParts">構成部品一覧</param>
public sealed record PartsListRegistration(
    string ItemCode,
    string ItemName,
    IReadOnlyList<ConstituentPart> ConstituentParts);
