namespace Sample.Persistence.Entities;

/// <summary>品番マスタ (Item)。</summary>
public class Item
{
    /// <summary>品番。</summary>
    public string ItemCode { get; set; } = default!;

    /// <summary>品名。</summary>
    public string ItemName { get; set; } = default!;

    /// <summary>この品番を構成する部品構成表レコード。</summary>
    public ICollection<Bom> Boms { get; set; } = new List<Bom>();
}
