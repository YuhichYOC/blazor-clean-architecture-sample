namespace Sample.Persistence.Records;

/// <summary>
/// Material テーブル（部品を表す）のレコードの入れ物。
/// </summary>
public sealed class MaterialRecord
{
    /// <summary>部品品番（PK）</summary>
    public string ItemCode { get; set; } = string.Empty;

    /// <summary>部品品名</summary>
    public string ItemName { get; set; } = string.Empty;
}
