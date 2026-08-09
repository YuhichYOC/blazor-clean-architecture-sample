namespace Sample.Persistence.Entities;

/// <summary>部品マスタ (Material)。item_code が部品品番を表す。</summary>
public class Material
{
    /// <summary>部品品番。</summary>
    public string ItemCode { get; set; } = default!;

    /// <summary>部品品名。</summary>
    public string ItemName { get; set; } = default!;

    /// <summary>この部品を使用している部品構成表レコード。</summary>
    public ICollection<Bom> Boms { get; set; } = new List<Bom>();
}
