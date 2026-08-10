namespace Sample.Presentation.Forms;

/// <summary>
/// 追加モーダルの可変フォームモデル(UI 所有)。
///
/// ドメイン集約 <c>Sample.Domain.Item</c> は「登録を確定する瞬間」の妥当な状態しか表現しない
/// (生成できた時点で全不変条件を満たす)。一方この画面では + / - で行を足し引きする
/// “編集途中” の状態 —— 0件・部品品番の重複・空欄・所要量0 など —— が普通に発生する。
///
/// その不正になり得る途中状態を引き受けるのがこのフォームモデルの役目。
/// 検査はしない。登録時にドメイン集約へ写した瞬間、不変条件が検査される。
/// </summary>
public sealed class ItemFormModel
{
    public string ItemCode { get; set; } = "";
    public string ItemName { get; set; } = "";

    public List<ComponentFormModel> Components { get; } = new();

    public void AddComponentRow() => Components.Add(new ComponentFormModel());

    public void RemoveComponentRow(ComponentFormModel row) => Components.Remove(row);
}
