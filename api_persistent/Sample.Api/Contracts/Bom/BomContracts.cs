// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// BomContracts.cs
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

namespace Sample.Api.Contracts.Bom;

// ─────────────────────────────────────────────────────────────
// 新API独自の契約。Sample.Persistence.Dtos への参照は持たない(= 完全独立)。
// 形はクライアント側 DTO と構造的に一致させ、JSON(camelCase)で橋渡しする。
// これが実質のワイヤ契約であり、名前・型・null 許容の一致が前提。
//   ここを共有ライブラリ化すれば重複は消えるが「完全独立」はわずかに崩れる ← 設計判断として明示。
// ─────────────────────────────────────────────────────────────

/// <summary>一覧の1行(左外部結合のため部品側は null 許容)。</summary>
public record BomRowResponse(
    string ItemCode,
    string ItemName,
    string? MItemCode,
    string? MItemName,
    decimal? Requirement);

/// <summary>登録要求(品番1件＋構成部品)。サーバー側で1トランザクションとして扱う。</summary>
public record ItemRegistrationRequest(
    string ItemCode,
    string ItemName,
    IReadOnlyList<ComponentModel> Components);

public record ComponentModel(
    string MItemCode,
    string MItemName,
    decimal Requirement);

/// <summary>一括カスケード削除の要求本文。</summary>
public record DeleteItemsRequest(IReadOnlyList<string> ItemCodes);

/// <summary>孤児部品も削除する経路の要求本文。</summary>
public record DeleteWithMaterialsRequest(IReadOnlyList<string> MaterialCodes);
