namespace Sample.Domain.Entities;

/// <summary>
/// 構成部品の1行（部品構成表登録画面の「構成部品」テーブルの1行に対応）。
/// 登録時に Material（部品マスタ）と Bom（所要量）の元になる。
/// </summary>
/// <param name="MaterialCode">部品品番</param>
/// <param name="MaterialName">部品品名</param>
/// <param name="Requirement">所要量</param>
public sealed record ConstituentPart(
    string MaterialCode,
    string MaterialName,
    decimal Requirement);
