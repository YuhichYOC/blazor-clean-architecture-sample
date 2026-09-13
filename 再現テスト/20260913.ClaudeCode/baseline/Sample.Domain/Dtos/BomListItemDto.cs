namespace Sample.Domain.Dtos;

/// <summary>
/// 部品構成表登録画面の一覧に表示する1行分のデータ。
/// Bom を基準に Item・Material を結合した結果を表す（画面イメージ-画面のロード時 参照）。
/// 品番に対して構成部品（Bom 明細）が1件も無い場合を考慮し、部品側の項目は null になり得る。
/// </summary>
/// <param name="ItemCode">品番（Item.item_code）</param>
/// <param name="ItemName">品名（Item.item_name）</param>
/// <param name="MaterialCode">部品品番（Material.item_code）</param>
/// <param name="MaterialName">部品品名（Material.item_name）</param>
/// <param name="Requirement">所要量（Bom.requirement）</param>
public sealed record BomListItemDto(
    string ItemCode,
    string ItemName,
    string? MaterialCode,
    string? MaterialName,
    decimal? Requirement);
