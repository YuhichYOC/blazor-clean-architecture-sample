// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// BomRowDto.cs
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

namespace Sample.Persistence.Dtos;

/// <summary>
/// 画面グリッドの1行分。1 Bom レコード = 1 行。
/// 左外部結合のため、構成部品を持たない品番では部品側 (MItemCode/MItemName/Requirement) が null になり得る。
/// 同一品番の2行目以降で品番・品名を空欄表示するのは画面側の責務。
/// </summary>
public record BomRowDto(
    string ItemCode,        // 品番
    string ItemName,        // 品名
    string? MItemCode,      // 部品品番
    string? MItemName,      // 部品品名
    decimal? Requirement    // 所要量
);
