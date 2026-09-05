// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// ComponentFormModel.cs
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
/// 構成部品の入力1行(可変)。
///
/// 所要量は「未入力」と「0」を区別できるよう decimal?(nullable)にしている。
/// - 未入力(null): 入力欄を空のまま登録 → 登録時に 0 として渡し、ドメインの「所要量>0」で弾く(Image 8)。
/// - 0: 明示的に 0 → 同じくドメインで弾く。
/// いずれにせよ検査はドメイン側。ここは値を保持するだけ。
/// </summary>
public sealed class ComponentFormModel
{
    public string MaterialCode { get; set; } = "";
    public string MaterialName { get; set; } = "";
    public decimal? Requirement { get; set; }
}
