namespace Sample.Domain.Validation;

/// <summary>
/// 入力検証エラー1件分。
/// </summary>
/// <param name="Field">エラーとなった項目を識別するキー（<see cref="ValidationFields"/> 参照）</param>
/// <param name="Message">画面に表示するメッセージ</param>
/// <param name="ConstituentPartIndex">
/// 構成部品テーブルの何行目のエラーかを示すインデックス（0始まり）。
/// 品番・品名など行に属さないエラーの場合は null。
/// </param>
public sealed record ValidationError(
    string Field,
    string Message,
    int? ConstituentPartIndex = null);
