namespace Sample.Domain.Validation;

/// <summary>
/// <see cref="ValidationError.Field"/> に入る項目キーの定数。
/// プレゼンテーション層がエラー箇所（赤枠表示など）を特定するために使用する。
/// </summary>
public static class ValidationFields
{
    public const string ItemCode = nameof(ItemCode);
    public const string ItemName = nameof(ItemName);
    public const string ConstituentParts = nameof(ConstituentParts);
    public const string MaterialCode = nameof(MaterialCode);
    public const string MaterialName = nameof(MaterialName);
    public const string Requirement = nameof(Requirement);
}
