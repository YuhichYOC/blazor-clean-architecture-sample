// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// ItemFormModel.cs
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
