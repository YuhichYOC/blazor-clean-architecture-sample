namespace Sample.Domain;

/// <summary>
/// 構成部品(部品構成表の1行)。ある品番を構成する部品への参照と、その所要量。
/// Item 集約の内部要素であり、単独では存在しない。
///
/// 部品品名(MaterialName)は、登録時に部品マスタ(Material)へ書き込むために保持する。
/// 名称そのものは業務規則ではないが「空でないこと」は要求する。
/// (Material を独立した集約と見るなら、本来は MaterialCode の参照だけを持ち、
///  名称は Material 側の責務。ここでは登録ユースケースの都合で名称も同伴させている。
///  「Material は所有か共有か」の判断は保留中で、この同伴は暫定的な割り切り。)
/// </summary>
public sealed class ComponentLine
{
    public string MaterialCode { get; }   // 部品品番 (Material.item_code / Bom.m_item_code)
    public string MaterialName { get; }   // 部品品名 (Material.item_name)
    public Requirement Requirement { get; }

    public ComponentLine(string materialCode, string materialName, Requirement requirement)
    {
        if (string.IsNullOrWhiteSpace(materialCode))
            throw new DomainException("部品品番は必須です。");
        if (string.IsNullOrWhiteSpace(materialName))
            throw new DomainException("部品品名は必須です。");

        MaterialCode = materialCode;
        MaterialName = materialName;
        Requirement = requirement;
    }
}
