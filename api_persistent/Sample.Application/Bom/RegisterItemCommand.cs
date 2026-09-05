namespace Sample.Application.Registration;

/// <summary>
/// 品番登録の入力(modal で入力された生データ)。
///
/// これはまだ検証されていない“候補”に過ぎない。
/// 不変条件の検査はこのコマンドではなくドメイン集約(Item/ComponentLine/Requirement)が行う。
/// 所要量は UI 側で decimal へパース済みとする(数値形式・空欄などの入力形式エラーは UI の責務)。
/// </summary>
public sealed record RegisterItemCommand(
    string ItemCode,
    string ItemName,
    IReadOnlyList<RegisterItemComponent> Components);

/// <summary>登録入力の構成部品1行。</summary>
public sealed record RegisterItemComponent(
    string MaterialCode,
    string MaterialName,
    decimal Requirement);
