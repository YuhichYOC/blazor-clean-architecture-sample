// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// BomRow.cs
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

namespace Sample.Application.ReadModels;

/// <summary>
/// 部品構成表一覧の1行(読み取り専用の表示モデル)。
///
/// 左外部結合の右側(Bom/Material)は「品番はあるが構成部品が無い」場合に null になり得る
/// ため、部品側の3項目は nullable。
///
/// 注: 永続層の BomRowDto と形は重なるが、あえて Application 所有の型として定義している。
///     こうすると UI は Sample.Persistence を参照せずに済む(境界の独立)。
///     Sample.Shared を作る段で「この読み取りモデルを Shared へ移すか」を再検討する。
/// </summary>
public sealed record BomRow(
    string ItemCode,
    string ItemName,
    string? MaterialCode,
    string? MaterialName,
    decimal? Requirement);
