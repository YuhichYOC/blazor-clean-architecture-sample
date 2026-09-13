namespace Sample.Persistence.Records;

/// <summary>
/// Item テーブル（品番を表す）のレコードの入れ物。
/// </summary>
public sealed class ItemRecord
{
    /// <summary>品番（PK）</summary>
    public string ItemCode { get; set; } = string.Empty;

    /// <summary>品名</summary>
    public string ItemName { get; set; } = string.Empty;
}
