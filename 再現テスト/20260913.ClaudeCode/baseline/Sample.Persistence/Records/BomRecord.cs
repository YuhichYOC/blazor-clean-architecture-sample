namespace Sample.Persistence.Records;

/// <summary>
/// Bom テーブル（部品構成表）のレコードの入れ物。
/// </summary>
public sealed class BomRecord
{
    /// <summary>品番（PK, FK → Item.item_code）</summary>
    public string ItemCode { get; set; } = string.Empty;

    /// <summary>部品品番（PK, FK → Material.item_code）</summary>
    public string MItemCode { get; set; } = string.Empty;

    /// <summary>所要量</summary>
    public decimal Requirement { get; set; }
}
