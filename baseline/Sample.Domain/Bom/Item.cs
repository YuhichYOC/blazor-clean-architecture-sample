// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// Item.cs
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

namespace Sample.Domain;

/// <summary>
/// 品番(集約ルート)。品番マスタ1件と、それを構成する部品(構成部品)の集合。
///
/// 不変条件:
///   - 品番コード・品名が空でないこと。
///   - 構成部品を 1 件以上持つこと(0 件の品番は「生成された瞬間から不正」)。
///   - 同一集約内で部品品番が重複しないこと(Bom の複合PKに対応するドメイン規則)。
///
/// 生成方針:
///   この集約は「登録を確定する瞬間」に生成する。
///   modal 上で + / - しながら行を足し引きする “編集途中” の一時状態
///   (0 件・重複・未入力など)は UI 側の可変フォームモデルが受け持ち、この集約には持ち込まない。
///   → 生成できた時点で不変条件は必ず満たされている(常に妥当な状態でしか存在しない)。
///
/// 可変性:
///   登録ユースケースでは AddComponent 等の可変操作は不要なので不変(immutable)にしている。
///   将来 品番マスタメンテ で「既存品番の構成を編集する」要件が実際に出てきたら、
///   そのとき初めて不変条件を守る形の変更メソッドを足す(使用が形を教えるまで抽象は遅延)。
/// </summary>
public sealed class Item
{
    private readonly List<ComponentLine> _components;

    public string ItemCode { get; }   // 品番 (Item.item_code)
    public string ItemName { get; }   // 品名 (Item.item_name)
    public IReadOnlyList<ComponentLine> Components => _components;

    public Item(string itemCode, string itemName, IEnumerable<ComponentLine> components)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new DomainException("品番は必須です。");
        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("品名は必須です。");

        var list = components?.ToList() ?? new List<ComponentLine>();

        // 不変条件: 構成部品は 1 件以上
        if (list.Count == 0)
            throw new DomainException("構成部品は1件以上必要です。");

        // 不変条件: 部品品番の重複禁止
        var duplicated = list
            .GroupBy(c => c.MaterialCode)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicated.Count > 0)
            throw new DomainException(
                $"構成部品の部品品番が重複しています: {string.Join(", ", duplicated)}");

        ItemCode = itemCode;
        ItemName = itemName;
        _components = list;
    }
}
