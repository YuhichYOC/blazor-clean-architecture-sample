// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// RegisterItemCommand.cs
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

namespace Sample.Application.Registration;

/// <summary>
/// 品番登録の入力(modal で入力された生データ)。
///
/// これはまだ検証されていない“候補”に過ぎない。
/// 不変条件の検査はこのコマンドではなくドメイン集約(Item/ComponentLine/Requirement)が行う。
/// 所要量は UI 側で decimal へパース済みとする(数値形式・空欄などの入力形式エラーは UI の責務)。
/// </summary>
public sealed record RegisterItemCommand(
    string ItemCode,
    string ItemName,
    IReadOnlyList<RegisterItemComponent> Components);

/// <summary>登録入力の構成部品1行。</summary>
public sealed record RegisterItemComponent(
    string MaterialCode,
    string MaterialName,
    decimal Requirement);
