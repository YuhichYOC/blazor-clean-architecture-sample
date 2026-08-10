namespace Sample.Persistence.Entities;

/// <summary>
/// 部品構成表 (Bom)。
/// (item_code, m_item_code) の複合主キー。item_code は Item、m_item_code は Material への外部キー。
/// </summary>
public class Bom
{
    /// <summary>品番 (Item への FK)。</summary>
    public string ItemCode { get; set; } = default!;

    /// <summary>部品品番 (Material への FK)。</summary>
    public string MItemCode { get; set; } = default!;

    /// <summary>所要量。</summary>
    public decimal Requirement { get; set; }

    public Item Item { get; set; } = default!;
    public Material Material { get; set; } = default!;
}
