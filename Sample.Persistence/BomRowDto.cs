namespace Sample.Persistence.Dtos;

/// <summary>
/// 画面グリッドの1行分。1 Bom レコード = 1 行。
/// 左外部結合のため、構成部品を持たない品番では部品側 (MItemCode/MItemName/Requirement) が null になり得る。
/// 同一品番の2行目以降で品番・品名を空欄表示するのは画面側の責務。
/// </summary>
public record BomRowDto(
    string ItemCode,        // 品番
    string ItemName,        // 品名
    string? MItemCode,      // 部品品番
    string? MItemName,      // 部品品名
    decimal? Requirement    // 所要量
);
