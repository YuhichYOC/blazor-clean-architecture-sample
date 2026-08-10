namespace Sample.Domain;

/// <summary>
/// 所要量。1つの品番に対して構成部品が必要となる数量。
///
/// 不変条件: 0 より大きいこと。ゼロ・負の所要量は物理的に無意味。
/// DB の NUMBER(9,2) は「ストレージ上の精度・桁数」の都合であり、
/// 「正であること」は業務上の規則なのでドメイン層が引き受ける。
/// </summary>
public readonly record struct Requirement
{
    public decimal Value { get; }

    public Requirement(decimal value)
    {
        if (value <= 0m)
            throw new DomainException($"所要量は0より大きい必要があります(指定値: {value})。");

        Value = value;
    }

    public override string ToString() => Value.ToString();

    // 注意(reference note):
    //   record struct には引数なしの既定値 default(Requirement) が存在し、
    //   これはコンストラクタを経由しないため Value=0 のまま生成できてしまう。
    //   本サンプルでは「所要量は必ず ComponentLine 経由で明示生成される」前提なので許容。
    //   厳密に塞ぐなら class 化するか、生成経路をファクトリに限定する。
}
