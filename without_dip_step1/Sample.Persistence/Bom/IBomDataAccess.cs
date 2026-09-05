// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// IBomDataAccess.cs
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

using Sample.Persistence.Dtos;

namespace Sample.Persistence;

/// <summary>部品構成表登録画面のデータアクセス。</summary>
public interface IBomDataAccess
{
    /// <summary>画面ロード: 全 Bom を Item/Material と結合し、品番→部品品番でソートして取得。</summary>
    Task<IReadOnlyList<BomRowDto>> GetBomListAsync(CancellationToken ct = default);

    /// <summary>追加登録: Item 1件・Material N件・Bom N件を1トランザクションで挿入。</summary>
    Task RegisterAsync(ItemRegistrationDto registration, CancellationToken ct = default);

    /// <summary>削除: 指定品番の Item・その構成部品(Material)・紐づく Bom を1トランザクションで削除。</summary>
    Task DeleteItemsAsync(IEnumerable<string> itemCodes, CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetMaterialCodesOfItemAsync(string itemCode, CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetOrphanMaterialCodesAsync(string itemCode, CancellationToken ct = default);

    Task DeleteItemKeepMaterialsAsync(string itemCode, CancellationToken ct = default);

    Task DeleteItemWithMaterialsAsync(string itemCode, IEnumerable<string> materialCodes, CancellationToken ct = default);
}
