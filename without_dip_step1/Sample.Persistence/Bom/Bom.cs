// Copyright 2026 Yuichi Yoshii
//     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
// 
// Bom.cs
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

namespace Sample.Persistence.Entities;

/// <summary>
/// 部品構成表 (Bom)。
/// (item_code, m_item_code) の複合主キー。item_code は Item、m_item_code は Material への外部キー。
/// </summary>
public class Bom
{
    /// <summary>品番 (Item への FK)。</summary>
    public string ItemCode { get; set; } = default!;

    /// <summary>部品品番 (Material への FK)。</summary>
    public string MItemCode { get; set; } = default!;

    /// <summary>所要量。</summary>
    public decimal Requirement { get; set; }

    public Item Item { get; set; } = default!;
    public Material Material { get; set; } = default!;
}
